using MediatR;
using Microsoft.AspNetCore.Identity;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Auth;
using WaferJobs.Domain.Auth.Exceptions;

namespace WaferJobs.Features.Auth.ResetPassword;

public class ResetPasswordCommandHandler(
    UserManager<ApplicationUserEntity> userManager,
    ILogger<ResetPasswordCommandHandler> logger
) : IRequestHandler<ResetPasswordCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(
        ResetPasswordCommand command,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());

        if (user is null) throw new UserNotFoundException(command.UserId);

        var resetResult = await userManager.ResetPasswordAsync(user, command.Token, command.Password);

        if (!resetResult.Errors.Any())
        {
            logger.LogInformation("Reset password for user: {UserId}", user.Id);
            return Unit.Value;
        }

        var passwordResetError = resetResult.Errors.First();

        if (passwordResetError.Code == nameof(IdentityErrorDescriber.InvalidToken))
        {
            logger.LogWarning("User {UserId} provided invalid password reset token {token}", user.Id, command.Token);
            return AuthErrors.InvalidPasswordResetToken;
        }

        var errorMessages = string.Join(", ", resetResult.Errors.Select(e => e.Description));
        throw new InvalidOperationException($"Failed to reset password for user {user.Id}. Errors: {errorMessages}");
    }
}