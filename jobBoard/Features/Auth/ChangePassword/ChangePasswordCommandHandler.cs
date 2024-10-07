using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Auth.Exceptions;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Features.Auth.ChangePassword;

public class ChangePasswordCommandHandler(
    ICurrentUserService currentUserService,
    UserManager<ApplicationUserEntity> userManager,
    ILogger<ChangePasswordCommandHandler> logger
)
    : IRequestHandler<ChangePasswordCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) throw new UserNotFoundException(userId);

        var changeResult = await userManager.ChangePasswordAsync(user, command.CurrentPassword, command.NewPassword);

        if (!changeResult.Errors.Any())
        {
            logger.LogInformation("Changed password for user: {UserId}", userId);
            return Unit.Value;
        }

        var passwordUpdateError = changeResult.Errors.First();

        if (passwordUpdateError.Code == nameof(IdentityErrorDescriber.PasswordMismatch))
        {
            logger.LogWarning("User {UserId} provided an incorrect current password for password change.", userId);
            return AuthErrors.InvalidCurrentPassword;
        }

        var errorMessages = string.Join(", ", changeResult.Errors.Select(e => e.Description));
        throw new InvalidOperationException($"Failed to change password for user {userId}. Errors: {errorMessages}");
    }
}