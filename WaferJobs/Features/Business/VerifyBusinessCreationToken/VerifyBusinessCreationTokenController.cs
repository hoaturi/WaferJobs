using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Constants;

namespace WaferJobs.Features.Business.VerifyBusinessCreationToken;

[Tags("Business")]
[Route("api/businesses/creation/verify-token")]
[ApiController]
public class VerifyBusinessCreationTokenController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPost]
    public async Task<IActionResult> VerifyBusinessCreationToken([FromBody] VerifyBusinessCreationTokenQuery query)
    {
        var result = await sender.Send(query);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}