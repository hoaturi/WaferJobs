using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard;

public class SignOutCommandHandler(
    IJwtService jwtService,
    IDistributedCache cache,
    ILogger<SignOutCommandHandler> logger
) : IRequestHandler<SignOutCommand, Result<Unit, Error>>
{
    private readonly IJwtService _jwtService = jwtService;
    private readonly IDistributedCache _cache = cache;
    private readonly ILogger<SignOutCommandHandler> _logger = logger;

    /* If user has refresh token they will be treated as logged In
    and refresh token will be registered in the blacklist on log out */
    public async Task<Result<Unit, Error>> Handle(
        SignOutCommand request,
        CancellationToken cancellationToken
    )
    {
        var refreshToken = request.RefreshToken.Split(" ")[1];
        var key = CacheKeys.RevokedToken + refreshToken;

        var isTokenBlackListed = await _cache.GetStringAsync(key, cancellationToken);
        if (isTokenBlackListed is not null)
        {
            _logger.LogError("User tried to log out with already blacklisted token.");
            return Unit.Value;
        }

        var validationResult = await _jwtService.ValidateRefreshToken(refreshToken);
        if (validationResult is false)
        {
            _logger.LogError("User tried to log out with invalid refresh token.");
            return Unit.Value;
        }

        await BlacklistToken(refreshToken, cancellationToken);

        _logger.LogInformation(
            "Successfully logged user out. The refresh token was added to the blacklist."
        );

        return Unit.Value;
    }

    private async Task BlacklistToken(string token, CancellationToken cancellationToken)
    {
        var key = CacheKeys.RevokedToken + token;
        var jwtExpiration = _jwtService.GetExpiration(token);
        var dateTimeNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var cacheExpirationInSeconds = jwtExpiration - dateTimeNow;

        await _cache.SetStringAsync(
            key,
            RefreshTokenStatus.Revoked.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheExpirationInSeconds)
            },
            cancellationToken
        );
    }
}
