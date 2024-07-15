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
        [FromQuery(Name = "city")] string? city,
        [FromQuery(Name = "country")] string? country,
        [FromQuery(Name = "remoteOnly")] string? remoteOnly,
        [FromQuery(Name = "postedDate")] int? postedDate,
        [FromQuery(Name = "categories")] List<string>? categories,
        [FromQuery(Name = "employmentTypes")] List<string>? employmentTypes,
        [FromQuery(Name = "page")] int page = 1
    )
    {
        var result = await sender.Send(
            new GetJobPostListQuery(keyword, city, country, remoteOnly, postedDate, categories, employmentTypes, page)
        );

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}