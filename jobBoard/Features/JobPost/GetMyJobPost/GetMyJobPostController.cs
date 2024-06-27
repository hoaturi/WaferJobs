using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobPost.GetMyJobPost;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class GetMyJobPostController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpGet("me/{id:guid}")]
    public async Task<IActionResult> GetMyJobPost(Guid id)
    {
        var result = await sender.Send(new GetMyJobPostQuery(id));

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}