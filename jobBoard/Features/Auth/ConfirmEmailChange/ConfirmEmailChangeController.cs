using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Auth.ConfirmEmailChange;

[Tags("Auth")]
[Route("api/auth/email-change/confirm")]
[ApiController]
public class ConfirmEmailChangeController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ConfirmEmailChange(ConfirmEmailChangeCommand changeCommand)
    {
        var result = await sender.Send(changeCommand);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}