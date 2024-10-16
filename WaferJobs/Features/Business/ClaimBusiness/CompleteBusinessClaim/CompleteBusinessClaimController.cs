using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Constants;

namespace WaferJobs.Features.Business.ClaimBusiness.CompleteBusinessClaim;

[Tags("Business")]
[Route("api/businesses/claim/complete")]
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