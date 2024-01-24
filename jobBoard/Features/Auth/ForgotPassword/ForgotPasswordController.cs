using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Auth")]
[ApiController]
[Route("api/auth/forgot-password")]
public class ForgotPasswordController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await _sender.Send(command);

        if (!result.IsSuccess)
        {
            return this.HandleFailure(result.Error!);
        }

        return Ok();
    }
}
