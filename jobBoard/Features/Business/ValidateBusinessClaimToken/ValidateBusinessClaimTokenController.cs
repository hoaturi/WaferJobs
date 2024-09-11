using JobBoard.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.ValidateBusinessClaimToken;

[Tags("Business")]
[Route("api/businesses/claims/validate-token")]
[ApiController]
public class ValidateBusinessClaimTokenController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpGet]
    public async Task<IActionResult> ValidateBusinessClaimToken([FromQuery] string token)
    {
        var result = await sender.Send(new ValidateBusinessClaimTokenQuery(token));

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}