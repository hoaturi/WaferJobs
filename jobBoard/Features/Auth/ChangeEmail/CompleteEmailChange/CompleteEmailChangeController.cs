using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Auth.ChangeEmail.CompleteEmailChange;

[Tags("Auth")]
[Route("api/auth/change-email/confirm")]
[ApiController]
public class CompleteEmailChangeController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpPatch]
    public async Task<IActionResult> CompleteEmailChange([FromBody] CompleteEmailChangeCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}