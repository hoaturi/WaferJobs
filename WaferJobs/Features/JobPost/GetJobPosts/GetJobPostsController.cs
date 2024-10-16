using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Features.JobPost.GetJobPosts.GetHomeJobPosts;

namespace WaferJobs.Features.JobPost.GetJobPosts;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class GetJobPostsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetJobPosts(
        [FromQuery(Name = "keyword")] string? keyword,
        [FromQuery(Name = "city")] int? city,
        [FromQuery(Name = "country")] int? country,
        [FromQuery(Name = "experienceLevels")] List<int>? experienceLevels,
        [FromQuery(Name = "remoteOnly")] bool? remoteOnly,
        [FromQuery(Name = "postedDate")] int? postedDate,
        [FromQuery(Name = "categories")] List<int>? categories,
        [FromQuery(Name = "employmentTypes")] List<int>? employmentTypes,
        [FromQuery(Name = "minSalary")] int? minSalary,
        [FromQuery(Name = "take")] int take = 20,
        [FromQuery(Name = "page")] int page = 1
    )
    {
        var result = await sender.Send(
            new GetJobPostsQuery(keyword, city, country, experienceLevels, remoteOnly, postedDate, categories,
                employmentTypes, minSalary, take, page)
        );

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }

    [HttpGet("home")]
    public async Task<IActionResult> GetHomeJobPosts()
    {
        var result = await sender.Send(new GetHomeJobPostsQuery());

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}