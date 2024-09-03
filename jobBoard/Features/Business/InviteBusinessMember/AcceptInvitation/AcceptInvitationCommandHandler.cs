using Hangfire;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Business;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.EmailService;
using JobBoard.Infrastructure.Services.EmailService.Dtos;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Business.InviteBusinessMember.AcceptInvitation;

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

        if (invitation is null) return BusinessErrors.InvalidInvitationToken;

        var user = new ApplicationUserEntity
        {
            UserName = invitation.InviteeEmail,
            Email = invitation.InviteeEmail
        };

        var createResult = await userManager.CreateAsync(user, command.Password);

        if (createResult.Errors.Any(e => e.Code == nameof(IdentityErrorDescriber.DuplicateEmail)))
            return AuthErrors.EmailAlreadyInUse;

        await userManager.AddToRoleAsync(user, nameof(UserRoles.Business));

        var confirmToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmEmailDto = new ConfirmEmailDto(user.Email, user.Id, confirmToken);

        var newMembership = new BusinessMemberEntity
        {
            BusinessId = invitation.BusinessId,
            UserId = user.Id,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Title = command.Title
        };

        dbContext.BusinessMembers.Add(newMembership);
        invitation.IsAccepted = true;
        invitation.AcceptedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        backgroundJobClient.Enqueue<IEmailService>(x => x.SendEmailConfirmAsync(confirmEmailDto));

        logger.LogInformation("User {UserId} accepted invitation for business {BusinessId}", user.Id,
            invitation.BusinessId);

        return Unit.Value;
    }
}