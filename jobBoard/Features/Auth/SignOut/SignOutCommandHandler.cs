using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard;

public class SignOutCommandHandler(IJwtService jwtService, IDistributedCache cache)
    : IRequestHandler<SignOutCommand, Result<Unit, Error>>
{
    private readonly IJwtService _jwtService = jwtService;
    private readonly IDistributedCache _cache = cache;

    /* If user has refresh token they will be treated as logged In
    and refresh token will be registered in the blacklist on log out */
    public async Task<Result<Unit, Error>> Handle(
        SignOutCommand request,
        CancellationToken cancellationToken
    )
    {
        var refreshToken = request.RefreshToken.Split(" ")[1];

        var validationResult = await _jwtService.ValidateRefreshToken(refreshToken);

        if (validationResult is false)
        {
            return AuthErrors.InvalidRefreshToken;
        }

        var jwtExpiration = _jwtService.GetExpiration(refreshToken);
        var dateTimeNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var CacheExpirationInSeconds = jwtExpiration - dateTimeNow;

        var key = CacheKeys.RevokedToken + refreshToken;
        await _cache.SetStringAsync(
            key,
            RefreshTokenStatus.Revoked.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheExpirationInSeconds)
            },
            token: cancellationToken
        );

        return Unit.Value;
    }
}
