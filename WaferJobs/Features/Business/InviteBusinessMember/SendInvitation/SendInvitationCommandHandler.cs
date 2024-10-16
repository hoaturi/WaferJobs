using Hangfire;
using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Auth;
using WaferJobs.Domain.Business;
using WaferJobs.Domain.Business.Exceptions;
using WaferJobs.Infrastructure.Persistence;
using WaferJobs.Infrastructure.Services.CurrentUserService;
using WaferJobs.Infrastructure.Services.EmailService;
using WaferJobs.Infrastructure.Services.EmailService.Dtos;

namespace WaferJobs.Features.Business.InviteBusinessMember.SendInvitation;

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

        var membership = await dbContext.BusinessMemberships
                             .AsNoTracking()
                             .Where(x => x.UserId == userId && x.IsActive)
                             .FirstOrDefaultAsync(cancellationToken)
                         ?? throw new UserHasNoBusinessMembershipException(userId);

        if (!membership.IsAdmin) throw new BusinessMemberNotAdminException(membership.BusinessId, userId);

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
            userId,
            membership.Business.Name,
            $"{membership.FirstName} {membership.LastName}",
            newInvitation.Token,
            TokenConstants.ExpiresIn7Days
        );
        backgroundJobClient.Enqueue<IEmailService>(x => x.SendBusinessMemberInvitationAsync(emailDto));

        logger.LogInformation("Invitation for business {BusinessId} sent by user: {UserId}", membership.BusinessId,
            userId);

        return Unit.Value;
    }
}