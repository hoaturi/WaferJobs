using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.JwtService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Auth.SignIn;

public class SignInCommandHandler(
    AppDbContext dbContext,
    UserManager<ApplicationUserEntity> userManager,
    IJwtService jwtService,
    ILogger<SignInCommandHandler> logger)
    : IRequestHandler<SignInCommand, Result<SignInResponse, Error>>
{
    public async Task<Result<SignInResponse, Error>> Handle(
        SignInCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);

        if (user is null) return AuthErrors.InvalidCredentials;

        if (!await ValidateUserCredentials(user, command.Password))
        {
            logger.LogWarning("User {UserId} provided invalid credentials", user.Id);
            return AuthErrors.InvalidCredentials;
        }

        if (!user.EmailConfirmed)
        {
            logger.LogWarning("User {UserId} attempted to sign in with an unverified email", user.Id);
            return AuthErrors.EmailNotVerified;
        }

        var roles = await userManager.GetRolesAsync(user);
        var hasCompletedOnboarding = await CheckOnboardingStatus(user, roles, cancellationToken);

        var (accessToken, refreshToken) = jwtService.GenerateTokens(user, roles);

        logger.LogInformation("Authenticated user: {UserId}. Roles: {Roles}", user.Id, string.Join(", ", roles));

        var userDto = new UserDto(user.Id, user.Email!, roles.ToArray(), hasCompletedOnboarding);

        return new SignInResponse(userDto, accessToken, refreshToken);
    }

    private async Task<bool> ValidateUserCredentials(ApplicationUserEntity user, string password)
    {
        var isCorrectPassword = await userManager.CheckPasswordAsync(user, password);

        return isCorrectPassword;
    }

    private async Task<bool> CheckOnboardingStatus(ApplicationUserEntity user, IList<string> roles,
        CancellationToken cancellationToken)
    {
        if (roles.Contains(nameof(UserRoles.JobSeeker)))
        {
            var hasCompletedOnboarding = await dbContext.JobSeekers.AsNoTracking()
                .AnyAsync(x => x.UserId == user.Id, cancellationToken);
            return hasCompletedOnboarding;
        }

        if (roles.Contains(nameof(UserRoles.Business)))
        {
            var hasCompletedOnboarding = await dbContext.BusinessMemberships.AsNoTracking()
                .AnyAsync(x => x.UserId == user.Id, cancellationToken);
            return hasCompletedOnboarding;
        }

        return false;
    }
}