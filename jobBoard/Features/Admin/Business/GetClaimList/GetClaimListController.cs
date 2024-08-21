using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Admin.Business.GetClaimList;

[Tags("Admin")]
[Route("api/admin/businesses/claims")]
[ApiController]
public class GetClaimListController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Admin))]
    [HttpGet]
    public async Task<IActionResult> GetClaimList([FromQuery] string? status)
    {
        var result = await sender.Send(new GetClaimListQuery(status));

        return result.IsSuccess
            ? Ok(result.Value)
            : this.HandleError(result.Error);
    }
}