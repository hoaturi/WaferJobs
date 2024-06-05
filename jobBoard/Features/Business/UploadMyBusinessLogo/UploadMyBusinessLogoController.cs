using JobBoard.Common.Attributes;
using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.UploadMyBusinessLogo;

[Tags("Business")]
[ApiController]
[Route("api/businesses")]
public class UploadMyBusinessLogoController(ISender sender) : ControllerBase
{
    [HttpPost("me/logo")]
    [Authorize(nameof(UserRoles.Business))]
    // Limit the size of the uploaded file to 1MB
    [RequestFormLimits(MultipartBodyLengthLimit = 1024 * 1024)]
    public async Task<IActionResult> UploadLogo([FromForm] [ValidateImageFile] IFormFile file)
    {
        var command = new UploadMyBusinessLogoCommand(file);

        var result = await sender.Send(command);

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok();
    }
}