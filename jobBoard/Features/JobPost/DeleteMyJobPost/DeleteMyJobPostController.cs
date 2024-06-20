using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobPost.DeleteMyJobPost;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class DeleteMyJobPostController(ISender sender) : ControllerBase
{
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteMyJobPost(Guid id)
    {
        var result = await sender.Send(new DeleteMyJobPostCommand(id));

        return result.IsSuccess ? Ok() : this.HandleError(result.Error);
    }
}