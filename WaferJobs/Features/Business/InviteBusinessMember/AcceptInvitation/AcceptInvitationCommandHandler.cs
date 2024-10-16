using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Auth;
using WaferJobs.Domain.Business;
using WaferJobs.Infrastructure.Persistence;
using WaferJobs.Infrastructure.Services.EmailService;
using WaferJobs.Infrastructure.Services.EmailService.Dtos;

namespace WaferJobs.Features.Business.InviteBusinessMember.AcceptInvitation;

public class AcceptInvitationCommandHandler(
    AppDbContext dbContext,
    UserManager<ApplicationUserEntity> userManager,
    IBackgroundJobClient backgroundJobClient,
    ILogger<AcceptInvitationCommandHandler> logger)
    : IRequestHandler<AcceptInvitationCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(AcceptInvitationCommand command, CancellationToken cancellationToken)
    {
        var invitation = await dbContext.BusinessMemberInvitations.FirstOrDefaultAsync(
            x =>
                x.Token == command.Token && !x.IsAccepted && x.ExpiresAt >= DateTime.UtcNow,
            cancellationToken);

        if (invitation is null)
        {
            logger.LogWarning("User attempted to accept invitation with invalid token: {Token}", command.Token);
            return BusinessErrors.InvalidInvitationToken;
        }

        var user = new ApplicationUserEntity
        {
            UserName = invitation.InviteeEmail,
            Email = invitation.InviteeEmail
        };

        var createResult = await userManager.CreateAsync(user, command.Password);

        if (!createResult.Succeeded)
        {
            if (createResult.Errors.All(e => e.Code == nameof(IdentityErrorDescriber.DuplicateEmail)))
            {
                logger.LogWarning("User attempted to accept invitation with email that already exists.");
                return AuthErrors.EmailAlreadyInUse;
            }

            var errorMessages = string.Join(", ", createResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user. Errors: {errorMessages}");
        }

        var addRoleResult = await userManager.AddToRoleAsync(user, nameof(UserRoles.Business));

        if (!addRoleResult.Succeeded)
        {
            var errorMessages = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to add user to role. Errors: {errorMessages}");
        }

        var confirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmEmailDto = new ConfirmEmailDto(user.Email, user.Id, confirmToken);

        var newMembership = new BusinessMembershipEntity
        {
            BusinessId = invitation.BusinessId,
            UserId = user.Id,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Title = command.Title
        };

        dbContext.BusinessMemberships.Add(newMembership);
        invitation.IsAccepted = true;
        invitation.AcceptedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        backgroundJobClient.Enqueue<IEmailService>(x => x.SendEmailConfirmAsync(confirmEmailDto));

        logger.LogInformation("User {UserId} accepted invitation for business: {BusinessId}", user.Id,
            invitation.BusinessId);

        return Unit.Value;
    }
}