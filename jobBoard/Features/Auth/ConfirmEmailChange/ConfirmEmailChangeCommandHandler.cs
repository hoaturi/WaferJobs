using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Auth.Exceptions;
using JobBoard.Domain.Common.Exceptions;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Auth.ConfirmEmailChange;

public class ConfirmEmailChangeCommandHandler(
    AppDbContext dbContext,
    UserManager<ApplicationUserEntity> userManager,
    ICurrentUserService currentUserService,
    ILogger<ConfirmEmailChangeCommandHandler> logger
)
    : IRequestHandler<ConfirmEmailChangeCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(ConfirmEmailChangeCommand command,
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
            throw new TooManyVerificationAttemptsException("email change", userId);

        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException(userId);

        var setEmailResult = await userManager.SetEmailAsync(user, changeRequest.NewEmail);
        var setUserNameResult = await userManager.SetUserNameAsync(user, changeRequest.NewEmail);

        if (!setEmailResult.Succeeded || !setUserNameResult.Succeeded)
            throw new InvalidOperationException($"Failed to update user email with id {userId}");

        changeRequest.IsVerified = true;
        changeRequest.VerifiedAt = DateTime.UtcNow;

        var unusedClaims = await dbContext.BusinessClaimRequests
            .Where(x => x.ClaimantUserId == userId && !x.IsUsed)
            .ToListAsync(cancellationToken);

        dbContext.BusinessClaimRequests.RemoveRange(unusedClaims);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Email changed successfully for user {UserId}", userId);

        return Unit.Value;
    }
}