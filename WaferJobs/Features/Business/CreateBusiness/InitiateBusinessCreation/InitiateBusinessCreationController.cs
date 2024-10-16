using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Constants;

namespace WaferJobs.Features.Business.CreateBusiness.InitiateBusinessCreation;

[Tags("Business")]
[Route("api/businesses/creation/initiate")]
[ApiController]
public class InitiateBusinessCreationController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPost]
    public async Task<IActionResult> InitiateBusinessCreation([FromBody] InitiateBusinessCreationCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}