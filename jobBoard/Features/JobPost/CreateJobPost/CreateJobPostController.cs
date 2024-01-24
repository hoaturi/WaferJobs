using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class CreateJobPostController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [Authorize(policy: RolePolicy.Business)]
    [HttpPost]
    public async Task<IActionResult> CreateJobPost(CreateJobPostCommand command)
    {
        var result = await _sender.Send(command);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }
}
