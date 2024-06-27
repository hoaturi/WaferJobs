using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Auth.ChangePassword;

[Tags("Auth")]
[ApiController]
[Route("api/auth")]
public class ChangePasswordController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess ? Ok() : this.HandleError(result.Error);
    }
}