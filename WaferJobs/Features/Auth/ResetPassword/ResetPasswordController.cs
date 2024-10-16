using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WaferJobs.Features.Auth.ResetPassword;

[Tags("Auth")]
[ApiController]
[Route("api/auth/reset-password")]
public class ResetPasswordController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ResetPassword(
        [FromQuery] Guid userId,
        [FromQuery] string token,
        [FromBody] ResetPasswordRequestDto requestDto
    )
    {
        var command = new ResetPasswordCommand(userId, token, requestDto.Password, requestDto.ConfirmPassword);

        var result = await sender.Send(command);

        return result.IsSuccess ? Ok() : this.HandleError(result.Error!);
    }
}