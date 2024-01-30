using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Business")]
[ApiController]
[Route("api/business")]
public class UploadMyBusinessLogoController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpPost("/me/logo")]
    [Authorize(RolePolicy.Business)]
    [RequestFormLimits(MultipartBodyLengthLimit = 1024 * 1024 * 5)]
    public async Task<IActionResult> UploadLogo([FromForm] IFormFile file)
    {
        var command = new UploadMyBusinessLogoCommand(file);

        var result = await _sender.Send(command);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Ok();
    }
}
