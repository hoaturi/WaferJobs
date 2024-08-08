using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobMetric.IncrementApplyCount;

[Tags("Job Metric")]
[ApiController]
[Route("/api/job-metric")]
public class IncrementApplyCountController(ISender sender) : ControllerBase
{
    [HttpPost("{id:guid}")]
    public async Task<IActionResult> IncrementApplyClick([FromRoute] Guid id)
    {
        var result = await sender.Send(new IncrementApplyCountCommand(id));

        return result.IsSuccess ? Ok() : this.HandleError(result.Error);
    }
}