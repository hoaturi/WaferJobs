using JobBoard.Common.Constants;
using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard.Features.Auth.SignOut;

public class SignOutCommandHandler(
    IJwtService jwtService,
    IDistributedCache cache,
    ILogger<SignOutCommandHandler> logger)
    : IRequestHandler<SignOutCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(
        SignOutCommand command,
        CancellationToken cancellationToken
    )
    {
        var cacheKey = CacheKeys.RevokedToken + command.RefreshToken;

        var isTokenRevoked = await cache.GetStringAsync(cacheKey, cancellationToken);
        if (isTokenRevoked is not null)
        {
            logger.LogWarning("User tried to log out with an already revoked refresh token: {RefreshToken}",
                command.RefreshToken);
            return Unit.Value;
        }

        var isTokenValid = await jwtService.ValidateToken(command.RefreshToken, JwtTypes.RefreshToken);
        if (!isTokenValid)
        {
            logger.LogWarning("User tried to log out with an invalid refresh token: {RefreshToken}",
                command.RefreshToken);
            return Unit.Value;
        }

        await RevokeRefreshToken(command.RefreshToken, cancellationToken);

        logger.LogInformation("Successfully logged user out. The refresh token: {RefreshToken} was revoked.",
            command.RefreshToken);

        return Unit.Value;
    }

    private async Task RevokeRefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.RevokedToken + refreshToken;
        var tokenExpiration = jwtService.GetExpiration(refreshToken);
        var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var cacheExpirationInSeconds = tokenExpiration - currentTimestamp;

        await cache.SetStringAsync(
            cacheKey,
            RefreshTokenStatus.Revoked.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheExpirationInSeconds)
            },
            cancellationToken
        );
    }
}