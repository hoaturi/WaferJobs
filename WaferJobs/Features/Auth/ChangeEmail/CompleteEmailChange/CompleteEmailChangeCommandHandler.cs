using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Auth;
using WaferJobs.Domain.Auth.Exceptions;
using WaferJobs.Infrastructure.Persistence;
using WaferJobs.Infrastructure.Services.CurrentUserService;

namespace WaferJobs.Features.Auth.ChangeEmail.CompleteEmailChange;

public class CompleteEmailChangeCommandHandler(
    AppDbContext dbContext,
    UserManager<ApplicationUserEntity> userManager,
    ICurrentUserService currentUserService,
    ILogger<CompleteEmailChangeCommandHandler> logger
)
    : IRequestHandler<CompleteEmailChangeCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(CompleteEmailChangeCommand command,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var changeRequest = await dbContext.EmailChangeRequests
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsVerified && x.ExpiresAt > DateTime.UtcNow,
                cancellationToken) ?? throw new EmailChangeRequestNotFoundException(userId);

        switch (changeRequest.Attempts)
        {
            case <= PinConstants.MaxPinAttempts when !changeRequest.Pin.Equals(command.Pin):
                changeRequest.Attempts++;
                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogWarning("User {UserId} provided incorrect pin for email change. Attempts: {AttemptsCount}",
                    userId,
                    changeRequest.Attempts);
                return AuthErrors.InvalidChangeEmailPin;
            case >= PinConstants.MaxPinAttempts:
                logger.LogWarning("User {UserId} reached max email change pin attempts", userId);
                return AuthErrors.MaxPinAttemptsReached;
        }

        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException(userId);

        var setEmailResult = await userManager.SetEmailAsync(user, changeRequest.NewEmail);
        var setUserNameResult = await userManager.SetUserNameAsync(user, changeRequest.NewEmail);
        user.EmailConfirmed = true;

        var errorMessages = ExtractIdentityErrors(setEmailResult, setUserNameResult);

        if (!string.IsNullOrEmpty(errorMessages))
            throw new InvalidOperationException(
                $"Failed to update email or username for user: {userId} Errors: {errorMessages}");

        changeRequest.IsVerified = true;
        changeRequest.VerifiedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Completed email change for user: {UserId}", userId);

        return Unit.Value;
    }

    private static string ExtractIdentityErrors(IdentityResult setEmailResult, IdentityResult setUserNameResult)
    {
        var errors = new List<string>();

        if (!setEmailResult.Succeeded)
            errors.AddRange(setEmailResult.Errors.Select(e => e.Description));

        if (!setUserNameResult.Succeeded)
            errors.AddRange(setUserNameResult.Errors.Select(e => e.Description));

        return errors.Count != 0 ? string.Join(", ", errors) : string.Empty;
    }
}