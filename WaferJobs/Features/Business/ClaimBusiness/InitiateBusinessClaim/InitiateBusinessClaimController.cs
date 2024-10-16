using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Constants;

namespace WaferJobs.Features.Business.ClaimBusiness.InitiateBusinessClaim;

[Tags("Business")]
[Route("api/businesses/{businessId:guid}/claim/initiate")]
[ApiController]
public class InitiateBusinessClaimController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPost]
    public async Task<IActionResult> InitiateBusinessClaim([FromRoute] Guid businessId)
    {
        var result = await sender.Send(new InitiateBusinessClaimCommand(businessId));

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}