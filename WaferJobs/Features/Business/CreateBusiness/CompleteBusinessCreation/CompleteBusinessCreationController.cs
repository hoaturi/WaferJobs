using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Constants;

namespace WaferJobs.Features.Business.CreateBusiness.CompleteBusinessCreation;

[Tags("Business")]
[Route("api/businesses/creation/complete")]
[ApiController]
public class CompleteBusinessCreationController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPost]
    public async Task<IActionResult> CreateBusiness([FromQuery] string token, [FromBody] CreateBusinessRequestDto dto)
    {
        var result = await sender.Send(new CompleteBusinessCreationCommand(token, dto));

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}