using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Auth")]
[ApiController]
[Route("api/auth/sign-out")]
public class SignOutController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> SignOut([FromHeader] string Authorization)
    {
        var result = await _sender.Send(new SignOutCommand(Authorization));

        if (!result.IsSuccess)
        {
            return this.HandleFailure(result.Error!);
        }

        return Ok();
    }
}
