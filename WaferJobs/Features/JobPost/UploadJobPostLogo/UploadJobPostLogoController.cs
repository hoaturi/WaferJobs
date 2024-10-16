using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Attributes;

namespace WaferJobs.Features.JobPost.UploadJobPostLogo;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs/logo")]
public class UploadJobPostLogoController(ISender sender) : ControllerBase
{
    [HttpPost]
    // Limit the size of the uploaded file to 5MB
    [RequestFormLimits(MultipartBodyLengthLimit = 1024 * 1024 * 5)]
    public async Task<IActionResult> UploadLogo([FromForm] [ValidateImageFile] IFormFile file)
    {
        var command = new UploadJobPostLogoCommand(file);

        var result = await sender.Send(command);

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok(result.Value);
    }
}