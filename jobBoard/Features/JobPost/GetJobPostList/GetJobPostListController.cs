using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class GetJobPostListController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

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
        var result = await _sender.Send(
            new GetJobPostListQuery(
                Keyword: keyword,
                Country: country,
                Remote: remote,
                Categories: categories,
                EmploymentTypes: employmentTypes,
                Page: page
            )
        );

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }
}
