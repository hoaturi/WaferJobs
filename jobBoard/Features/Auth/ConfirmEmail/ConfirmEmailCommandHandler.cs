using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Auth.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Features.Auth.ConfirmEmail;

public class ConfirmEmailCommandHandler(
    UserManager<ApplicationUserEntity> userManager,
    ILogger<ConfirmEmailCommandHandler> logger
) : IRequestHandler<ConfirmEmailCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(ConfirmEmailCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());

        if (user is null)
        {
            logger.LogWarning("User {UserId} not found", command.UserId);
            throw new UserNotFoundException(command.UserId);
        }

        var result = await userManager.ConfirmEmailAsync(user, command.Token);

        if (result.Succeeded)
        {
            logger.LogInformation("Email confirmed for user with id: {UserId}", user.Id);
            return Unit.Value;
        }

        if (result.Errors.Any(e => e.Code == "InvalidToken"))
            throw new InvalidEmailConfirmTokenException(user.Id);

        throw new InvalidOperationException("Unexpected error occurred while confirming email");
    }
}