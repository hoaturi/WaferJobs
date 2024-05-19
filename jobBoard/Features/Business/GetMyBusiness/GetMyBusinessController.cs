using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.GetMyBusiness;

[Tags("Business")]
[ApiController]
[Route("api/businesses")]
public class GetMyBusinessController(ISender sender) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyBusiness()
    {
        var result = await sender.Send(new GetMyBusinessQuery());

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok(result.Value);
    }
}