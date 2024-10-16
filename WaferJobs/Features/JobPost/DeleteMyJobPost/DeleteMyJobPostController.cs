using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Constants;

namespace WaferJobs.Features.JobPost.DeleteMyJobPost;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class DeleteMyJobPostController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteMyJobPost(Guid id)
    {
        var result = await sender.Send(new DeleteMyJobPostCommand(id));

        return result.IsSuccess ? NoContent() : this.HandleError(result.Error);
    }
}