using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Auth.ResetPassword;

[Tags("Auth")]
[ApiController]
[Route("api/auth/reset-password")]
public class ResetPasswordController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ResetPassword(
        [FromQuery] Guid userId,
        [FromQuery] string token,
        [FromBody] ResetPasswordRequest request
    )
    {
        var command = new ResetPasswordCommand(userId, token, request.Password, request.ConfirmPassword);

        var result = await sender.Send(command);

        return result.IsSuccess ? Ok() : this.HandleError(result.Error!);
    }
}