using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class GetJobPostController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetJobPost(Guid id)
    {
        var result = await _sender.Send(new GetJobPostQuery(id));

        return Ok(result);
    }
}
