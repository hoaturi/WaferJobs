using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Features.Auth.ResetPassword;

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

        if (user is null)
        {
            logger.LogWarning("Failed to find user with id: {UserId}", command.UserId);
            return AuthErrors.UserNotFound;
        }

        var resetResult = await userManager.ResetPasswordAsync(user, command.Token, command.Password);

        if (!resetResult.Succeeded)
        {
            logger.LogWarning("Invalid reset token for user with id: {UserId}", command.UserId);
            return AuthErrors.InvalidToken;
        }

        logger.LogInformation("Password reset successful for user with id: {UserId}", user.Id);
        return Unit.Value;
    }
}