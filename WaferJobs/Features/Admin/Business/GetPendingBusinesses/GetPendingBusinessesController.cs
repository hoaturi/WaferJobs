using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Constants;

namespace WaferJobs.Features.Admin.Business.GetPendingBusinesses;

[Tags("Admin")]
[Route("api/admin/businesses/pending")]
[ApiController]
public class GetPendingBusinessesController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Admin))]
    [HttpGet]
    public async Task<IActionResult> GetPendingBusinesses()
    {
        var result = await sender.Send(new GetPendingBusinessesQuery());

        return result.IsSuccess
            ? Ok(result.Value)
            : this.HandleError(result.Error);
    }
}