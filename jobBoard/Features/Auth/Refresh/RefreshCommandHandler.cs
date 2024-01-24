using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard;

public class RefreshCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtService jwtService,
    IDistributedCache cache
) : IRequestHandler<RefreshCommand, Result<RefreshResponse, Error>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IDistributedCache _cache = cache;

    public async Task<Result<RefreshResponse, Error>> Handle(
        RefreshCommand request,
        CancellationToken cancellationToken
    )
    {
        var refreshToken = request.RefreshToken.Split(" ")[1];
        var key = CacheKeys.RevokedToken + refreshToken;
        var isRevoked = await _cache.GetStringAsync(key, cancellationToken);

        if (isRevoked is not null)
        {
            return AuthErrors.InvalidRefreshToken;
        }

        var validationResult = await _jwtService.ValidateRefreshToken(refreshToken);

        if (validationResult is false)
        {
            return AuthErrors.InvalidRefreshToken;
        }

        var userId = _jwtService.GetUserId(refreshToken);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return AuthErrors.InvalidRefreshToken;
        }

        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = _jwtService.GenerateAccessToken(user, [..roles]);

        return new RefreshResponse(accessToken);
    }
}
