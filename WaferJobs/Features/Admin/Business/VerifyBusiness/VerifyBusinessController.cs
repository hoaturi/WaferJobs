using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Constants;

namespace WaferJobs.Features.Admin.Business.VerifyBusiness;

[Tags("Admin")]
[Route("api/admin/businesses/{businessId:guid}/verify")]
[ApiController]
public class VerifyBusinessController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Admin))]
    [HttpPatch]
    public async Task<IActionResult> VerifyBusiness([FromRoute] Guid businessId,
        [FromBody] VerifyBusinessRequestDto dto)
    {
        var result = await sender.Send(new VerifyBusinessCommand(businessId, dto.IsApproved));

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}