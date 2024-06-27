using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobPost.GetMyJobPostList;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class GetMyJobPostListController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyJobPostList(
        [FromQuery(Name = "status")] string? status,
        [FromQuery(Name = "page")] int page = 1)
    {
        var result = await sender.Send(new GetMyJobPostListQuery(status, page));

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}