using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard;

public class RefreshCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtService jwtService,
    IDistributedCache cache,
    ILogger<RefreshCommandHandler> logger
) : IRequestHandler<RefreshCommand, Result<RefreshResponse, Error>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IDistributedCache _cache = cache;
    private readonly ILogger<RefreshCommandHandler> _logger = logger;

    public async Task<Result<RefreshResponse, Error>> Handle(
        RefreshCommand request,
        CancellationToken cancellationToken
    )
    {
        var refreshToken = request.RefreshToken.Split(" ")[1];

        var key = CacheKeys.RevokedToken + refreshToken;

        var isTokenRevoked = await _cache.GetStringAsync(key, cancellationToken);
        if (isTokenRevoked is not null)
        {
            _logger.LogWarning("The refresh token is already revoked.");
            return AuthErrors.InvalidRefreshToken;
        }

        var validationResult = await _jwtService.ValidateRefreshToken(refreshToken);
        if (validationResult is false)
        {
            _logger.LogWarning("The refresh token is invalid.");
            return AuthErrors.InvalidRefreshToken;
        }

        var userId = _jwtService.GetUserId(refreshToken);
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            _logger.LogError("User with id: {UserId} not found", userId);
            return AuthErrors.InvalidRefreshToken;
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);

        _logger.LogInformation("Successfully refreshed access token for user: {userId}", user.Id);

        return new RefreshResponse(accessToken);
    }
}
