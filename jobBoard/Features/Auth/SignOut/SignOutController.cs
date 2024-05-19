using JobBoard.Common.Extensions;
using JobBoard.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Auth.SignOut;

[Tags("Auth")]
[ApiController]
[Route("api/auth/signout")]
public class SignOutController(ISender sender) : ControllerBase
{
    [HttpPost]
    public new async Task<IActionResult> SignOut()
    {
        var refreshToken = HttpContext.Request.Cookies["refresh_token"];

        if (refreshToken is null) return this.HandleError(AuthErrors.InvalidRefreshToken);

        var result = await sender.Send(new SignOutCommand(refreshToken));

        if (!result.IsSuccess) return this.HandleError(result.Error);

        DeleteRefreshTokenCookie();

        return Ok();
    }

    private void DeleteRefreshTokenCookie()
    {
        HttpContext.Response.Cookies.Delete(
            "refresh_token",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                Path = "/"
            }
        );
    }
}