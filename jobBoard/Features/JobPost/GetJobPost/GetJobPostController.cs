using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobPost.GetJobPost;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class GetJobPostController(ISender sender) : ControllerBase
{
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetJobPost(string slug)
    {
        var result = await sender.Send(new GetJobPostQuery(slug));

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok(result.Value);
    }
}