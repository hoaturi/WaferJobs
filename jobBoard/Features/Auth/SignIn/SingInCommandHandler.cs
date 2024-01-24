using MediatR;
using Microsoft.AspNetCore.Identity;

namespace JobBoard;

public class SingInCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtService tokenService
) : IRequestHandler<SignInCommand, Result<SignInResponse, Error>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IJwtService _tokenService = tokenService;

    public async Task<Result<SignInResponse, Error>> Handle(
        SignInCommand request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return AuthErrors.InvalidCredentials;
        }

        var result = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!result)
        {
            return AuthErrors.InvalidCredentials;
        }

        var roles = await _userManager.GetRolesAsync(user!);

        var accessToken = _tokenService.GenerateAccessToken(user, [.. roles]);
        var refreshToken = _tokenService.GenerateRefreshToken(user);

        var userResponse = new UserResponse(user.Id, user.Email!, [..roles]);

        return new SignInResponse(userResponse, accessToken, refreshToken);
    }
}
