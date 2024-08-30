using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.BusinessClaim.CreateClaimantMember;

[Tags("Business")]
[Route("api/businesses/claims/complete")]
public class CreateClaimantMemberController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateClaimantMember([FromBody] CreateClaimantMemberCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}