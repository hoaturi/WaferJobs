using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Auth.ResendConfirmation;

[Tags("Auth")]
[Route("api/auth/resend-confirmation")]
[ApiController]
public class ResendConfirmationController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}