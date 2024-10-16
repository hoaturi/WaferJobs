using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Constants;

namespace WaferJobs.Features.JobPost.GetMyJobPost;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class GetMyJobPostController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpGet("me/{slug}")]
    public async Task<IActionResult> GetMyJobPost(string slug)
    {
        var result = await sender.Send(new GetMyJobPostQuery(slug));

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}