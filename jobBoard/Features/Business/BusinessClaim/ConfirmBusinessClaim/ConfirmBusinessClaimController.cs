using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.BusinessClaim.ConfirmBusinessClaim;

[Tags("Business")]
[Route("api/business/claim/confirm")]
[ApiController]
public class ConfirmBusinessClaimController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPost]
    public async Task<IActionResult> ConfirmBusinessClaim([FromBody] ConfirmBusinessClaimCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}