using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
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
            logger.LogInformation("User {UserId} changed their password", userId);
            return Unit.Value;
        }

        var passwordUpdateError = changeResult.Errors.First();

        if (passwordUpdateError.Code == "PasswordMismatch")
            return AuthErrors.InvalidCurrentPassword;

        throw new ChangePasswordFailedException(userId);
    }
}