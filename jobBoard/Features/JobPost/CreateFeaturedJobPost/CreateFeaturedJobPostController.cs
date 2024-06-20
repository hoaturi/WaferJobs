using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobPost.CreateFeaturedJobPost;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class CreateFeaturedJobPostController(ISender sender) : ControllerBase
{
    [HttpPost("/new")]
    [Authorize(nameof(UserRoles.Business))]
    public async Task<IActionResult> CreateJobPost([FromBody] CreateFeaturedJobPostCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }

    [HttpPost("guest")]
    public async Task<IActionResult> CreateGuestJobPost([FromBody] CreateFeaturedJobPostCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}