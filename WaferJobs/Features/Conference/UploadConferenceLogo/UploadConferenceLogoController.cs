using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Attributes;

namespace WaferJobs.Features.Conference.UploadConferenceLogo;

[Tags("Conference")]
[Route("/api/conferences/logo")]
[ApiController]
public class UploadConferenceLogoController(ISender sender) : ControllerBase
{
    [HttpPost]
    // Limit the size of the uploaded file to 5MB
    [RequestFormLimits(MultipartBodyLengthLimit = 1024 * 1024 * 5)]
    public async Task<IActionResult> UploadLogo([FromForm] [ValidateImageFile] IFormFile file)
    {
        var result = await sender.Send(new UploadConferenceLogoCommand(file));

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}