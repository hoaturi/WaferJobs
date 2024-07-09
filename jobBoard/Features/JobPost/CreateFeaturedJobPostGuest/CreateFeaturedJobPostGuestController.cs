using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobPost.CreateFeaturedJobPostGuest;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs/new")]
public class CreateFeaturedJobPostGuestController(ISender sender) : ControllerBase
{
    [HttpPost("guest")]
    public async Task<IActionResult> CreateJobPost([FromBody] CreateFeaturedJobPostGuestCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}