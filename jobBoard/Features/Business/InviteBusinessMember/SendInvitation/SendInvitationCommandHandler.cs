using Hangfire;
using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Business;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.EmailService;
using JobBoard.Infrastructure.Services.EmailService.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.InviteBusinessMember.SendInvitation;

public class SendInvitationCommandHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    IBackgroundJobClient backgroundJobClient,
    ILogger<SendInvitationCommandHandler> logger) : IRequestHandler<SendInvitationCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(SendInvitationCommand command, CancellationToken cancellationToken)
    {
        var isInviteeEmailAlreadyInUse =
            await dbContext.Users.AsNoTracking().AnyAsync(x => x.Email == command.Email, cancellationToken);
        if (isInviteeEmailAlreadyInUse) return AuthErrors.EmailAlreadyInUse;

        var userId = currentUserService.GetUserId();

        var membership = await dbContext.BusinessMembers
                             .AsNoTracking()
                             .Where(x => x.UserId == userId && x.IsActive && x.IsAdmin)
                             .FirstOrDefaultAsync(cancellationToken)
                         ?? throw new BusinessMembershipNotFoundException(userId);

        var newInvitation = new BusinessMemberInvitationEntity
        {
            BusinessId = membership.BusinessId,
            InviterId = userId,
            InviteeEmail = command.Email,
            Token = Guid.NewGuid().ToBase64String(),
            IsAccepted = false,
            AcceptedAt = null,
            ExpiresAt = DateTime.UtcNow.AddDays(TokenConstants.ExpiresIn7Days)
        };

        dbContext.BusinessMemberInvitations.Add(newInvitation);
        await dbContext.SaveChangesAsync(cancellationToken);

        var emailDto = new BusinessMemberInvitationEmailDto(
            command.Email,
            membership.BusinessId,
            membership.Business.Name,
            $"{membership.FirstName} {membership.LastName}",
            newInvitation.Token,
            TokenConstants.ExpiresIn7Days
        );
        backgroundJobClient.Enqueue<IEmailService>(x => x.SendBusinessMemberInvitationAsync(emailDto));

        logger.LogInformation("New member invitation sent for business {BusinessId}. Invited by user {UserId}.",
            membership.BusinessId, userId);

        return Unit.Value;
    }
}