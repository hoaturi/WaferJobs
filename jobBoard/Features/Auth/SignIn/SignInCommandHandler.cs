using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
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

        if (user is null)
        {
            logger.LogWarning("Failed to find user with email: {Email}", command.Email);
            return AuthErrors.InvalidCredentials;
        }

        var isCorrectPassword = await userManager.CheckPasswordAsync(user, command.Password);

        if (!isCorrectPassword)
        {
            logger.LogWarning("Invalid password for user with id: {UserId}", user.Id);
            return AuthErrors.InvalidCredentials;
        }

        var roles = await userManager.GetRolesAsync(user);

        var (accessToken, refreshToken) = jwtService.GenerateTokens(user, roles);

        logger.LogInformation("User: {Id} logged in successfully", user.Id);

        var userResponse = new UserResponse(user.Id, user.Email!, roles.ToArray());

        return new SignInResponse(userResponse, accessToken, refreshToken);
    }
}