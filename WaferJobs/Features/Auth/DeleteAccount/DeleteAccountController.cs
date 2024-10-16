using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WaferJobs.Features.Auth.DeleteAccount;

[Tags("Auth")]
[Route("api/auth/account")]
[ApiController]
public class DeleteAccountController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.Error);
    }
}