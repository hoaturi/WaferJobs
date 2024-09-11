using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.ValidateBusinessCreationToken;

[Tags("Business")]
[Route("api/businesses/creation/validate-token")]
[ApiController]
public class ValidateBusinessCreationTokenController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ValidateBusinessCreationToken([FromQuery] string token)
    {
        var result = await sender.Send(new ValidateBusinessCreationTokenQuery(token));

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}