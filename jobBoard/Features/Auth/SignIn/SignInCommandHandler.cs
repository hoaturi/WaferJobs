using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Services.JwtService;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard.Features.Auth.SignIn;

public class SignInCommandHandler(
    UserManager<ApplicationUserEntity> userManager,
    IJwtService jwtService,
    ILogger<SignInCommandHandler> logger
) : IRequestHandler<SignInCommand, Result<SignInResponse, Error>>
{
    public async Task<Result<SignInResponse, Error>> Handle(
        SignInCommand command,
        CancellationToken cancellationToken
    )
    {
        var user = await userManager.FindByEmailAsync(command.Email);

        if (user is null) return AuthErrors.InvalidCredentials;

        var isCorrectPassword = await userManager.CheckPasswordAsync(user, command.Password);

        if (!isCorrectPassword)
        {
            logger.LogWarning("Failed login attempt: Incorrect password for user {Email}", user.Email);
            return AuthErrors.InvalidCredentials;
        }

        if (!user.EmailConfirmed)
        {
            logger.LogWarning("Failed login attempt: Email not verified for user {Email}", user.Email);
            return AuthErrors.EmailNotVerified;
        }

        var roles = await userManager.GetRolesAsync(user);

        var (accessToken, refreshToken) = jwtService.GenerateTokens(user, roles);

        logger.LogInformation("Successful login: User {Email} authenticated", user.Email);

        var userResponse = new UserResponse(user.Id, user.Email!, roles.ToArray());

        return new SignInResponse(userResponse, accessToken, refreshToken);
    }
}