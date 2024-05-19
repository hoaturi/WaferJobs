using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Auth.ForgotPassword;

[Tags("Auth")]
[ApiController]
[Route("api/auth/forgot-password")]
public class ForgotPasswordController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var result = await sender.Send(command);

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok();
    }
}