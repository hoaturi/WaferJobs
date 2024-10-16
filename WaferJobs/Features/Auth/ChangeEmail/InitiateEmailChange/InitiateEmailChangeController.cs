using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WaferJobs.Features.Auth.ChangeEmail.InitiateEmailChange;

[Tags("Auth")]
[Route("api/auth/change-email")]
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