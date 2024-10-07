using JobBoard.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.VerifyBusinessClaimToken;

[Tags("Business")]
[Route("api/businesses/claim/verify-token")]
[ApiController]
public class VerifyBusinessClaimTokenController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPost]
    public async Task<IActionResult> VerifyBusinessClaimToken([FromBody] VerifyBusinessClaimTokenQuery query)
    {
        var result = await sender.Send(query);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}