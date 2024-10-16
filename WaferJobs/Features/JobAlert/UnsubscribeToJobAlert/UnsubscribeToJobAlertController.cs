using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WaferJobs.Features.JobAlert.UnsubscribeToJobAlert;

[Tags("Job Alert")]
[ApiController]
[Route("api/job-alerts")]
public class UnsubscribeToJobAlertController(ISender sender) : ControllerBase
{
    [HttpDelete("unsubscribe/{token}")]
    public async Task<IActionResult> Unsubscribe([FromRoute] string token)
    {
        var result = await sender.Send(new UnsubscribeToJobAlertCommand(token));

        return result.IsSuccess
            ? NoContent()
            : this.HandleError(result.Error);
    }
}