using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Auth.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Auth.ChangeEmail.CompleteEmailChange;

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

        if (!changeRequest.Pin.Equals(command.Pin))
        {
            changeRequest.Attempts++;
            await dbContext.SaveChangesAsync(cancellationToken);
            return AuthErrors.InvalidEmailChangePin;
        }

        if (changeRequest.Attempts >= PinConstants.MaxPinAttempts)
            throw new TooManyVerificationAttemptsException(userId);

        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException(userId);

        var setEmailResult = await userManager.SetEmailAsync(user, changeRequest.NewEmail);
        var setUserNameResult = await userManager.SetUserNameAsync(user, changeRequest.NewEmail);
        user.EmailConfirmed = true;

        if (!setEmailResult.Succeeded || !setUserNameResult.Succeeded)
            throw new InvalidOperationException($"Failed to update user email with id {userId}");

        changeRequest.IsVerified = true;
        changeRequest.VerifiedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Email changed successfully for user {UserId}", userId);

        return Unit.Value;
    }
}