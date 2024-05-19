using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.UploadMyBusinessLogo;

[Tags("Business")]
[ApiController]
[Route("api/business")]
public class UploadMyBusinessLogoController(ISender sender) : ControllerBase
{
    [HttpPost("me/logo")]
    [Authorize(nameof(UserRoles.Business))]
    [RequestFormLimits(MultipartBodyLengthLimit = 1024 * 1024 * 5)]
    public async Task<IActionResult> UploadLogo([FromForm] IFormFile file)
    {
        var command = new UploadMyBusinessLogoCommand(file);

        var result = await sender.Send(command);

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok();
    }
}