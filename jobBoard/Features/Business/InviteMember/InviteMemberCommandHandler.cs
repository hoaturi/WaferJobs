using System.Web;
using Hangfire;
using JobBoard.Common.Extensions;
using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.EmailService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.InviteMember;

public class InviteMemberCommandHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    IBackgroundJobClient backgroundJobClient,
    ILogger<InviteMemberCommandHandler> logger) : IRequestHandler<InviteMemberCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(InviteMemberCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();
        var userEmail = currentUserService.GetUserEmail();

        if (userEmail == command.InviteeEmail)
            return BusinessErrors.SelfInvitationNotAllowed;

        var member = await GetBusinessMemberAsync(userId, cancellationToken);

        if (!member.IsAdmin)
            throw new InsufficientBusinessPermissionException(member.UserId);

        if (await IsInviteeAlreadyMemberAsync(command.InviteeEmail, cancellationToken))
        {
            logger.LogWarning("Invitee {InviteeEmail} already has a membership", command.InviteeEmail);
            return BusinessErrors.UserCannotBeInvited;
        }

        var newInvitation = CreateBusinessMemberInvitation(command, member);
        await dbContext.BusinessMemberInvitations.AddAsync(newInvitation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Business member invitation to {businessName} created for {InviteeEmail}",
            member.Business.Name, command.InviteeEmail);

        QueueEmailInvitation(command, member, newInvitation.Token);
        Console.WriteLine(newInvitation.Token);

        return Unit.Value;
    }

    private async Task<BusinessMemberEntity> GetBusinessMemberAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.BusinessMembers
                   .AsNoTracking()
                   .Include(x => x.Business)
                   .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken)
               ?? throw new UserNotBusinessMemberException(userId);
    }

    private async Task<bool> IsInviteeAlreadyMemberAsync(string inviteeEmail, CancellationToken cancellationToken)
    {
        return await dbContext.BusinessMembers
            .AsNoTracking()
            .AnyAsync(bm => bm.User.Email == inviteeEmail, cancellationToken);
    }

    private BusinessMemberInvitationEntity CreateBusinessMemberInvitation(InviteMemberCommand command,
        BusinessMemberEntity member)
    {
        return new BusinessMemberInvitationEntity
        {
            BusinessId = member.BusinessId,
            InviterId = member.Id,
            InviterName = $"{member.FirstName} {member.LastName}",
            InviteeEmail = command.InviteeEmail,
            Token = Guid.NewGuid().ToBase64String(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsActive = true
        };
    }

    private void QueueEmailInvitation(InviteMemberCommand command, BusinessMemberEntity member, string token)
    {
        backgroundJobClient.Enqueue<EmailService>(x => x.SendBusinessMemberInvitationAsync(
            new BusinessMemberInvitationDto(
                command.InviteeEmail,
                member.Business.Name,
                member.FirstName,
                HttpUtility.UrlEncode(token)
            )));
    }
}