using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.GetBusiness;

[Tags("Business")]
[ApiController]
[Route("api/businesses")]
public class GetBusinessController(ISender sender) : ControllerBase
{
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBusiness(string slug)
    {
        var result = await sender.Send(new GetBusinessQuery(slug));

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok(result.Value);
    }
}