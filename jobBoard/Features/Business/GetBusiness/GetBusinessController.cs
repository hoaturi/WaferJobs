using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.GetBusiness;

[Tags("Business")]
[ApiController]
[Route("api/businesses")]
public class GetBusinessController(ISender sender) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBusiness(Guid id)
    {
        var result = await sender.Send(new GetBusinessQuery(id));

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok(result.Value);
    }
}