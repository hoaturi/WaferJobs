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
        [FromQuery(Name = "category")] int categoryId,
        [FromQuery(Name = "country")] int countryId,
        [FromQuery(Name = "employmentType")] int employmentTypeId,
        [FromQuery(Name = "page")] int page = 1
    )
    {
        var result = await _sender.Send(
            new GetJobPostListQuery(keyword, categoryId, countryId, employmentTypeId, page)
        );

        if (!result.IsSuccess)
        {
            return NotFound(result.Error);
        }

        return Ok(result.Value);
    }
}
