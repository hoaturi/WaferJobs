using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Admin.Business.VerifyBusinessClaim;

[Tags("Admin")]
[Route("api/admin/businesses/claim/verify")]
[ApiController]
public class VerifyBusinessClaimController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Admin))]
    [HttpPost("{claimId:guid}")]
    public async Task<IActionResult> VerifyBusinessClaim([FromRoute] Guid claimId,
        [FromBody] VerifyBusinessClaimDto dto)
    {
        var result = await sender.Send(new VerifyBusinessClaimCommand(claimId, dto));

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}