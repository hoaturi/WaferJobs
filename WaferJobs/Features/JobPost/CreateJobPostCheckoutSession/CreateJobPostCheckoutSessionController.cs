using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Constants;

namespace WaferJobs.Features.JobPost.CreateJobPostCheckoutSession;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class CreateJobPostCheckoutSessionController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPost("{id:guid}/checkout")]
    public async Task<IActionResult> CreateJobPostCheckoutSession(
        [FromRoute] Guid id
    )
    {
        var result = await sender.Send(new CreateJobPostCheckoutSessionCommand(id));

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}