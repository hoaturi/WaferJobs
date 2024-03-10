using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JobBoard;

[Tags("Auth")]
[ApiController]
[Route("api/auth/signin")]
public class SignInController(IOptions<JwtOptions> jwtOptions, ISender sender) : ControllerBase
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> SignIn([FromBody] SignInCommand command)
    {
        var result = await _sender.Send(command);

        if (!result.IsSuccess)
        {
            return this.HandleFailure(result.Error!);
        }

        var refreshToken = result.Value.RefreshToken;

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            // Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(double.Parse(_jwtOptions.RefreshExpires)),
        };

        HttpContext.Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);

        return Ok(new { user = result.Value.User, accessToken = result.Value.AccessToken });
    }
}
