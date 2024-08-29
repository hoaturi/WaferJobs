using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.InitiateBusinessClaim;

[Tags("Business")]
[Route("api/businesses/{businessId:guid}/claim")]
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