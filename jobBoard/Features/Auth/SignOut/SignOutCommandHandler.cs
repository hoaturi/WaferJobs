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
            logger.LogWarning("User attempted to sign out with a revoked refresh token: {RefreshToken}",
                command.RefreshToken);
            return Unit.Value;
        }

        var isTokenValid = await jwtService.ValidateToken(command.RefreshToken, JwtTypes.RefreshToken);
        if (!isTokenValid)
        {
            logger.LogWarning("User attempted to sign out with an invalid refresh token: {RefreshToken}",
                command.RefreshToken);
            return Unit.Value;
        }

        await jwtService.RevokeRefreshToken(command.RefreshToken, cancellationToken);

        logger.LogInformation("Signed out user with refresh token: {refreshToken}", command.RefreshToken);
        return Unit.Value;
    }
}