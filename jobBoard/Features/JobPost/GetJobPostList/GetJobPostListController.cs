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
        [FromQuery(Name = "city")] int? city,
        [FromQuery(Name = "country")] int? country,
        [FromQuery(Name = "experienceLevel")] int? experienceLevel,
        [FromQuery(Name = "remoteOnly")] bool? remoteOnly,
        [FromQuery(Name = "postedDate")] int? postedDate,
        [FromQuery(Name = "categories")] List<int>? categories,
        [FromQuery(Name = "employmentTypes")] List<int>? employmentTypes,
        [FromQuery(Name = "featuredOnly")] bool? featuredOnly,
        [FromQuery(Name = "currency")] int? currency,
        [FromQuery(Name = "minSalary")] int? minSalary,
        [FromQuery(Name = "maxSalary")] int? maxSalary,
        [FromQuery(Name = "take")] int take = 20,
        [FromQuery(Name = "page")] int page = 1
    )
    {
        var result = await sender.Send(
            new GetJobPostListQuery(keyword, city, country, experienceLevel, remoteOnly, postedDate, categories,
                employmentTypes, featuredOnly, currency, minSalary, maxSalary, take, page)
        );

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}