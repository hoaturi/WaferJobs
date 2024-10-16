using MediatR;
using Microsoft.AspNetCore.Identity;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Auth;
using WaferJobs.Domain.Auth.Exceptions;
using WaferJobs.Infrastructure.Services.CurrentUserService;

namespace WaferJobs.Features.Auth.DeleteAccount;

public class DeleteAccountCommandCommandHandler(
    ICurrentUserService currentUserService,
    UserManager<ApplicationUserEntity> userManager,
    ILogger<DeleteAccountCommandCommandHandler> logger) : IRequestHandler<DeleteAccountCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException(userId);

        var isPasswordCorrect = await userManager.CheckPasswordAsync(user, command.Password);

        if (!isPasswordCorrect)
        {
            logger.LogWarning("User {UserId} provided incorrect password for account deletion", userId);
            return AuthErrors.InvalidCurrentPassword;
        }

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to delete user {userId}. Errors: {errorMessages}");
        }

        logger.LogInformation("Deleted account for user: {UserId}", userId);

        return Unit.Value;
    }
}