using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobPost.GetJobPost;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class GetJobPostController(ISender sender) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetJobPost(Guid id)
    {
        var result = await sender.Send(new GetJobPostQuery(id));

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok(result.Value);
    }
}