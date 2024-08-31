using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.ClaimBusiness.CompleteBusinessClaim;

[Tags("Business")]
[Route("api/businesses/claims/complete")]
public class CompleteBusinessClaimController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPost]
    public async Task<IActionResult> CompleteBusinessClaim([FromQuery] string token,
        [FromBody] CompleteBusinessClaimRequestDto dto)
    {
        var result = await sender.Send(new CompleteBusinessClaimCommand(token, dto));

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}