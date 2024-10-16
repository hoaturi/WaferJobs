using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WaferJobs.Features.JobAlert.SubscribeToJobAlert;

[Tags("Job Alert")]
[ApiController]
[Route("api/job-alerts")]
public class SubscribeToJobAlertController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SubscribeToJobAlert(
        [FromBody] SubscribeToJobAlertCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}