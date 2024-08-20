using System.Web;
using Hangfire;
using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Persistence.Utils;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.EmailService;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace JobBoard.Features.Business.InviteMember;

public class InviteMemberCommandHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    IBackgroundJobClient backgroundJobClient,
    IEntityConstraintChecker constraintChecker,
    ILogger<InviteMemberCommandHandler> logger) : IRequestHandler<InviteMemberCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(InviteMemberCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var userId = currentUserService.GetUserId();

            var member = await dbContext.BusinessMembers
                .Include(x => x.Business)
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (member is null) throw new UserNotBusinessMemberException(userId);
            if (!member.IsAdmin) throw new InsufficientBusinessPermissionException(member.Id);

            var newInvitation = new BusinessMemberInvitationEntity
            {
                BusinessId = member.BusinessId,
                InviterId = member.Id,
                InviteeEmail = command.InviteeEmail,
                Token = HttpUtility.UrlEncode(Convert.ToBase64String(Guid.NewGuid().ToByteArray())),
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await dbContext.BusinessMemberInvitations.AddAsync(newInvitation, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Business member invitation to {businessName} created for {InviteeEmail}",
                command.InviteeEmail, member.Business.Name);

            backgroundJobClient.Enqueue<EmailService>(x => x.SendBusinessMemberInvitationAsync(
                new BusinessMemberInvitationDto(
                    command.InviteeEmail,
                    member.Business.Name,
                    member.FirstName,
                    newInvitation.Token
                )));

            return Unit.Value;
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is not PostgresException pgEx ||
                !constraintChecker.IsUniqueConstraintViolation<BusinessMemberInvitationEntity>(
                    nameof(BusinessMemberInvitationEntity.InviteeEmail),
                    pgEx.SqlState, pgEx.ConstraintName)) throw;

            logger.LogWarning("Business member invitation to {InviteeEmail} already exists", command.InviteeEmail);

            return BusinessErrors.InvitationAlreadyExists;
        }
    }
}