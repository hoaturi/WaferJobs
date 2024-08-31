using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Infrastructure.Services.JwtService;
using MediatR;

namespace JobBoard.Features.Auth.SignOut;

public class SignOutCommandHandler(
    IJwtService jwtService,
    ILogger<SignOutCommandHandler> logger)
    : IRequestHandler<SignOutCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(
        SignOutCommand command,
        CancellationToken cancellationToken
    )
    {
        var isTokenRevoked = await jwtService.IsRefreshTokenRevoked(command.RefreshToken, cancellationToken);
        if (isTokenRevoked)
        {
            logger.LogWarning("Attempt to log out with an already revoked refresh token");
            return Unit.Value;
        }

        var isTokenValid = await jwtService.ValidateToken(command.RefreshToken, JwtTypes.RefreshToken);
        if (!isTokenValid)
        {
            logger.LogWarning("Attempt to log out with an invalid refresh token");
            return Unit.Value;
        }

        await jwtService.RevokeRefreshToken(command.RefreshToken, cancellationToken);

        logger.LogInformation("User successfully logged out and refresh token revoked");

        return Unit.Value;
    }
}