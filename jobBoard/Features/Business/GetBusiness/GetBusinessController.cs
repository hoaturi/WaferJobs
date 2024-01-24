using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Business")]
[ApiController]
[Route("api/businesses")]
public class GetBusinessController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBusiness(Guid id)
    {
        var result = await _sender.Send(new GetBusinessQuery(id));

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }
}
