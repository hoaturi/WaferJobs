using JobBoard.Common.Extensions;
using JobBoard.Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JobBoard.Features.Auth.SignIn;

[Tags("Auth")]
[ApiController]
[Route("api/auth/signin")]
public class SignInController(IOptions<JwtOptions> jwtOptions, ISender sender) : ControllerBase
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    [HttpPost]
    public async Task<IActionResult> SignIn([FromBody] SignInCommand command)
    {
        var result = await sender.Send(command);

        if (!result.IsSuccess) return this.HandleError(result.Error);

        var refreshToken = result.Value.RefreshToken;
        var accessToken = result.Value.AccessToken;

        HttpContext.Response.Cookies.Append(
            "refresh_token",
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(double.Parse(_jwtOptions.RefreshExpireDays)),
                Path = "/"
            }
        );

        return Ok(new { user = result.Value.User, accessToken });
    }
}