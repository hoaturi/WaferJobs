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
    public async Task<IActionResult> Refresh([FromHeader] string Authorization)
    {
        var result = await _sender.Send(new RefreshCommand(Authorization));

        if (!result.IsSuccess)
        {
            return this.HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }
}
