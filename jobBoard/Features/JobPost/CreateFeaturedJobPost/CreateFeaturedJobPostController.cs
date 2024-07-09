using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobPost.CreateFeaturedJobPost;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs/new")]
public class CreateFeaturedJobPostController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(nameof(UserRoles.Business))]
    public async Task<IActionResult> CreateJobPost([FromBody] CreateFeaturedJobPostCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}