using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Auth")]
[ApiController]
[Route("api/auth/refresh")]
public class RefreshController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> Refresh()
    {
        HttpContext.Request.Cookies.TryGetValue("refresh_token", out var refreshToken);

        var result = await _sender.Send(new RefreshCommand(refreshToken!));

        if (!result.IsSuccess)
        {
            return this.HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }
}
