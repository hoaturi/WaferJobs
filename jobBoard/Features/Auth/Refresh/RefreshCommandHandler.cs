using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Features.Auth.SignIn;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.JwtService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard.Features.Auth.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, Result<RefreshResponse, Error>>
{
    private readonly IDistributedCache _cache;
    private readonly AppDbContext _dbContext;
    private readonly IJwtService _jwtService;
    private readonly ILogger<RefreshCommandHandler> _logger;
    private readonly UserManager<ApplicationUserEntity> _userManager;

    public RefreshCommandHandler(
        UserManager<ApplicationUserEntity> userManager,
        IJwtService jwtService,
        IDistributedCache cache,
        ILogger<RefreshCommandHandler> logger,
        AppDbContext dbContext)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _cache = cache;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<RefreshResponse, Error>> Handle(
        RefreshCommand command,
        CancellationToken cancellationToken)
    {
        if (await IsTokenRevoked(command, cancellationToken))
        {
            _logger.LogWarning("Refresh token {RefreshToken} is already revoked.", command.RefreshToken);
            return AuthErrors.InvalidRefreshToken;
        }

        if (!await _jwtService.ValidateToken(command.RefreshToken, JwtTypes.RefreshToken))
        {
            _logger.LogWarning("Refresh token {RefreshToken} is invalid.", command.RefreshToken);
            return AuthErrors.InvalidRefreshToken;
        }

        var userId = _jwtService.GetUserId(command.RefreshToken).ToString();
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            _logger.LogError("User with id {UserId} not found.", userId);
            return AuthErrors.InvalidRefreshToken;
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var (newAccessToken, newRefreshToken) = _jwtService.GenerateTokens(user, userRoles);

        var hasCompletedOnboarding = await CheckOnboardingStatus(user, userRoles, cancellationToken);

        _logger.LogInformation("Access token refreshed for user ID: {UserId}", user.Id);

        var userResponse = new UserDto(user.Id, user.Email!, userRoles.ToArray(), hasCompletedOnboarding);
        return new RefreshResponse(userResponse, newAccessToken);
    }

    private async Task<bool> IsTokenRevoked(RefreshCommand command, CancellationToken cancellationToken)
    {
        var revokedTokenKey = CacheKeys.RevokedToken + command.RefreshToken;
        return await _cache.GetStringAsync(revokedTokenKey, cancellationToken) is not null;
    }

    private async Task<bool> CheckOnboardingStatus(ApplicationUserEntity user, IList<string> roles,
        CancellationToken cancellationToken)
    {
        if (roles.Contains(nameof(UserRoles.JobSeeker)))
            return await _dbContext.JobSeekers.AsNoTracking()
                .AnyAsync(x => x.UserId == user.Id, cancellationToken);

        if (roles.Contains(nameof(UserRoles.Business)))
            return await _dbContext.BusinessMemberships.AsNoTracking()
                .AnyAsync(x => x.UserId == user.Id, cancellationToken);

        return false;
    }
}