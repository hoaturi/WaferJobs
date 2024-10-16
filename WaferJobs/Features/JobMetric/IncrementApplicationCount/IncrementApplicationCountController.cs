using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WaferJobs.Features.JobMetric.IncrementApplicationCount;

[Tags("Job Metric")]
[ApiController]
[Route("/api/job-metrics/{id:guid}/application-count")]
public class IncrementApplicationCountController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> IncrementApplicationCount([FromRoute] Guid id)
    {
        var result = await sender.Send(new IncrementApplicationCountCommand(id));

        return result.IsSuccess ? Ok() : this.HandleError(result.Error);
    }
}