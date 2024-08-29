using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Auth.InitiateEmailChange;

[Tags("Auth")]
[Route("api/auth/email-change")]
[ApiController]
public class InitiateEmailChangeController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> InitiateEmailChange(InitiateEmailChangeCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}