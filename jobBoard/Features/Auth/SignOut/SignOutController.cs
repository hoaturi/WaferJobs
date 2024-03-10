using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Auth")]
[ApiController]
[Route("api/auth/signout")]
public class SignOutController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public new async Task<IActionResult> SignOut()
    {
        HttpContext.Request.Cookies.TryGetValue("refresh_token", out var refreshToken);

        var result = await _sender.Send(new SignOutCommand(refreshToken!));

        if (!result.IsSuccess)
        {
            return this.HandleFailure(result.Error!);
        }

        HttpContext.Response.Cookies.Delete("refresh_token");

        return Ok();
    }
}
