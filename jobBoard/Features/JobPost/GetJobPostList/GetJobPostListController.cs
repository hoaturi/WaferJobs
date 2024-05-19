using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobPost.GetJobPostList;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class GetJobPostListController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetJobPostList(
        [FromQuery(Name = "keyword")] string? keyword,
        [FromQuery(Name = "country")] string? country,
        [FromQuery(Name = "remote")] string? remote,
        [FromQuery(Name = "category")] List<string>? categories,
        [FromQuery(Name = "employmentType")] List<string>? employmentTypes,
        [FromQuery(Name = "page")] int page = 1
    )
    {
        var result = await sender.Send(
            new GetJobPostListQuery(keyword, country, remote, categories, employmentTypes, page)
        );

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok(result.Value);
    }
}