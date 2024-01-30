using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Business")]
[ApiController]
[Route("api/businesses")]
public class GetMyBusinessController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [Authorize(RolePolicy.Business)]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyBusiness()
    {
        var result = await _sender.Send(new GetMyBusinessQuery());

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Ok(result.Value);
    }
}
