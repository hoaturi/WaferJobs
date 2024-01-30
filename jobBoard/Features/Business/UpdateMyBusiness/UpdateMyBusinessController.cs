using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Business")]
[ApiController]
[Route("api/businesses")]
public class UpdateMyBusinessController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [Authorize(RolePolicy.Business)]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateBusiness([FromBody] UpdateMyBusinessCommand command)
    {
        var result = await _sender.Send(command);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Ok();
    }
}
