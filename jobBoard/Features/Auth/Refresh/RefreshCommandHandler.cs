using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Services.JwtService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard.Features.Auth.Refresh;

public class RefreshCommandHandler(
    UserManager<ApplicationUserEntity> userManager,
    IJwtService jwtService,
    IDistributedCache cache,
    ILogger<RefreshCommandHandler> logger
) : IRequestHandler<RefreshCommand, Result<RefreshResponse, Error>>
{
    public async Task<Result<RefreshResponse, Error>> Handle(
        RefreshCommand command,
        CancellationToken cancellationToken
    )
    {
        if (await IsTokenRevoked(command, cancellationToken))
        {
            logger.LogWarning("Refresh token {RefreshToken} is already revoked.", command.RefreshToken);
            return AuthErrors.InvalidRefreshToken;
        }

        if (!await jwtService.ValidateToken(command.RefreshToken, JwtTypes.RefreshToken))
        {
            logger.LogWarning("Refresh token {RefreshToken} is invalid.", command.RefreshToken);
            return AuthErrors.InvalidRefreshToken;
        }

        var userId = jwtService.GetUserId(command.RefreshToken).ToString();
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            logger.LogError("User with id {UserId} not found.", userId);
            return AuthErrors.InvalidRefreshToken;
        }

        var userRoles = await userManager.GetRolesAsync(user);
        var newAccessToken = jwtService.GenerateAccessToken(user, userRoles);

        logger.LogInformation("Access token refreshed for user: {UserId}", user.Id);

        return new RefreshResponse(newAccessToken);
    }

    private async Task<bool> IsTokenRevoked(RefreshCommand command, CancellationToken cancellationToken)
    {
        var revokedTokenKey = CacheKeys.RevokedToken + command.RefreshToken;
        return await cache.GetStringAsync(revokedTokenKey, cancellationToken) is not null;
    }
}