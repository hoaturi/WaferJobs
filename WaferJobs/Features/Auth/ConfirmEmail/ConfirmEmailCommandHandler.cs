using MediatR;
using Microsoft.AspNetCore.Identity;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Auth;
using WaferJobs.Domain.Auth.Exceptions;

namespace WaferJobs.Features.Auth.ConfirmEmail;

public class ConfirmEmailCommandHandler(
    UserManager<ApplicationUserEntity> userManager,
    ILogger<ConfirmEmailCommandHandler> logger
) : IRequestHandler<ConfirmEmailCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());

        if (user is null) throw new UserNotFoundException(command.UserId);

        var result = await userManager.ConfirmEmailAsync(user, command.Token);

        if (result.Succeeded)
        {
            logger.LogInformation("Confirmed email for user: {UserId}.", command.UserId);
            return Unit.Value;
        }

        if (result.Errors.All(e => e.Code != nameof(IdentityErrorDescriber.InvalidToken)))
        {
            var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(
                $"Failed to confirm email for user: {command.UserId}. Errors: {errorMessages}");
        }

        logger.LogWarning("User {UserId} provided an invalid token for email confirmation.", command.UserId);
        return AuthErrors.InvalidEmailConfirmationToken;
    }
}