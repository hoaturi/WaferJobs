using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Auth.Exceptions;
using JobBoard.Features.Auth.SignIn;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.JwtService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard.Features.Auth.Refresh;

public class RefreshCommandHandler(
    UserManager<ApplicationUserEntity> userManager,
    IJwtService jwtService,
    IDistributedCache cache,
    ILogger<RefreshCommandHandler> logger,
    AppDbContext dbContext)
    : IRequestHandler<RefreshCommand, Result<RefreshResponse, Error>>
{
    public async Task<Result<RefreshResponse, Error>> Handle(
        RefreshCommand command,
        CancellationToken cancellationToken)
    {
        if (await IsTokenRevoked(command, cancellationToken))
        {
            logger.LogWarning("User provided already revoked refresh token: {RefreshToken}", command.RefreshToken);
            return AuthErrors.InvalidRefreshToken;
        }

        if (!await jwtService.ValidateToken(command.RefreshToken, JwtTypes.RefreshToken))
        {
            logger.LogWarning("User provided an invalid refresh token: {RefreshToken}", command.RefreshToken);
            return AuthErrors.InvalidRefreshToken;
        }

        var userId = jwtService.GetUserId(command.RefreshToken).ToString();
        var user = await userManager.FindByIdAsync(userId);

        if (user is null) throw new UserNotFoundException(new Guid(userId));

        var userRoles = await userManager.GetRolesAsync(user);
        var (newAccessToken, _) = jwtService.GenerateTokens(user, userRoles);

        var hasCompletedOnboarding = await CheckOnboardingStatus(user, userRoles, cancellationToken);

        logger.LogInformation("Refresh access token for user: {UserId}", userId);

        var userResponse = new UserDto(user.Id, user.Email!, userRoles.ToArray(), hasCompletedOnboarding);
        return new RefreshResponse(userResponse, newAccessToken);
    }

    private async Task<bool> IsTokenRevoked(RefreshCommand command, CancellationToken cancellationToken)
    {
        var revokedTokenKey = CacheKeys.RevokedToken + command.RefreshToken;
        return await cache.GetStringAsync(revokedTokenKey, cancellationToken) is not null;
    }

    private async Task<bool> CheckOnboardingStatus(ApplicationUserEntity user, IList<string> roles,
        CancellationToken cancellationToken)
    {
        if (roles.Contains(nameof(UserRoles.JobSeeker)))
            return await dbContext.JobSeekers.AsNoTracking()
                .AnyAsync(x => x.UserId == user.Id, cancellationToken);

        if (roles.Contains(nameof(UserRoles.Business)))
            return await dbContext.BusinessMemberships.AsNoTracking()
                .AnyAsync(x => x.UserId == user.Id, cancellationToken);

        return false;
    }
}