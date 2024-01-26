using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard;

public class SignInCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtService jwtService,
    ILogger<SignInCommandHandler> logger
) : IRequestHandler<SignInCommand, Result<SignInResponse, Error>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtService _jwtService = jwtService;
    private readonly ILogger<SignInCommandHandler> _logger = logger;

    public async Task<Result<SignInResponse, Error>> Handle(
        SignInCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            _logger.LogInformation("User with the input email not found");
            return AuthErrors.InvalidCredentials;
        }

        var result = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!result)
        {
            _logger.LogInformation("Invalid password for user with id {UserId}", user.Id);
            return AuthErrors.InvalidCredentials;
        }

        var roles = await _userManager.GetRolesAsync(user);

        var (accessToken, refreshToken) = _jwtService.GenerateTokens(user, roles);

        _logger.LogInformation("User: {Id} logged in successfully", user.Id);

        var userResponse = new UserResponse(user.Id, user.Email!, [..roles]);
        return new SignInResponse(userResponse, accessToken, refreshToken);
    }
}
