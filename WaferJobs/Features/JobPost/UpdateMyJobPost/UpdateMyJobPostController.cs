using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Constants;

namespace WaferJobs.Features.JobPost.UpdateMyJobPost;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class UpdateMyJobPostController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPut("me/{id:guid}")]
    public async Task<IActionResult> UpdateMyJobPost([FromRoute] Guid id, [FromBody] UpdateMyJobPostDto dto)
    {
        var command = new UpdateMyJobPostCommand(
            id,
            dto
        );

        var result = await sender.Send(command);

        return result.IsSuccess ? Ok() : this.HandleError(result.Error);
    }
}