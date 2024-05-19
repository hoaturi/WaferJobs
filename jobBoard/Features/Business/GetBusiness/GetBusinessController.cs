using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.GetBusiness;

[Tags("Business")]
[ApiController]
[Route("api/businesses")]
public class GetBusinessController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBusiness(Guid id)
    {
        var result = await _sender.Send(new GetBusinessQuery(id));

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok(result.Value);
    }
}