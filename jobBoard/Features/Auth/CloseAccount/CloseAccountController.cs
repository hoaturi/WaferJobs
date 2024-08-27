using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Auth.CloseAccount;

[Tags("Auth")]
[Route("api/auth/close-account")]
[ApiController]
public class CloseAccountController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> CloseAccount([FromBody] CloseAccountCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.Error);
    }
}