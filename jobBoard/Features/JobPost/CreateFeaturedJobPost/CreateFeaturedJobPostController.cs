using JobBoard.Common.Attributes;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobPost.CreateFeaturedJobPost;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class CreateFeaturedJobPostController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ConditionalAuthorization]
    public async Task<IActionResult> CreateJobPost(CreateFeaturedJobPostCommand command)
    {
        var result = await sender.Send(command);

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok(result.Value);
    }
}