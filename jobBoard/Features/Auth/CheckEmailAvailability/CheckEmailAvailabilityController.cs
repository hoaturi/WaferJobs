using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Auth.CheckEmailAvailability;

[Tags("Auth")]
[ApiController]
[Route("api/auth/check-email")]
public class CheckEmailAvailabilityController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> CheckEmailAvailability([FromQuery] CheckEmailAvailabilityQuery query)
    {
        var result = await sender.Send(query);

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}