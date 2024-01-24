using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Auth")]
[ApiController]
[Route("api/auth/reset-password")]
public class ResetPasswordController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> ResetPassword(
        [FromQuery] Guid userId,
        [FromQuery] string token,
        [FromBody] ResetPasswordRequest body
    )
    {
        var command = new ResetPasswordCommand
        {
            UserId = userId,
            Token = token,
            Password = body.Password,
            ConfirmPassword = body.ConfirmPassword
        };

        var result = await _sender.Send(command);

        if (!result.IsSuccess)
        {
            return this.HandleFailure(result.Error!);
        }

        return Ok();
    }
}
