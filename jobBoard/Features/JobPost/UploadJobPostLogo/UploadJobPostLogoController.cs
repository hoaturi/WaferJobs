using JobBoard.Common.Attributes;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobPost.UploadJobPostLogo;

[Tags("Job Post")]
[ApiController]
[Route("api/jobs")]
public class UploadJobPostLogoController(ISender sender) : ControllerBase
{
    [HttpPost("new/logo")]
    // Limit the size of the uploaded file to 1MB
    [RequestFormLimits(MultipartBodyLengthLimit = 1024 * 1024)]
    public async Task<IActionResult> UploadLogo([FromForm] [ValidateImageFile] IFormFile file)
    {
        var command = new UploadJobPostLogoCommand(file);

        var result = await sender.Send(command);

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok(result.Value);
    }
}