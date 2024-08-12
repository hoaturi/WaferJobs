using JobBoard.Common.Attributes;
using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.UpdateMyBusinessLogo;

[Tags("Business")]
[ApiController]
[Route("api/businesses")]
public class UpdateMyBusinessLogoController(ISender sender) : ControllerBase
{
    [HttpPost("me/logo")]
    [Authorize(nameof(UserRoles.Business))]
    // Limit the size of the uploaded file to 5MB
    [RequestFormLimits(MultipartBodyLengthLimit = 1024 * 1024 * 5)]
    public async Task<IActionResult> UploadLogo([FromForm] [ValidateImageFile] IFormFile file)
    {
        var command = new UpdateMyBusinessLogoCommand(file);

        var result = await sender.Send(command);

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}