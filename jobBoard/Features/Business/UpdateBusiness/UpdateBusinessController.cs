using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Business")]
[ApiController]
[Route("api/businesses")]
public class UpdateBusinessController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [Authorize(policy: RolePolicy.Business)]
    [HttpPut]
    public async Task<IActionResult> UpdateBusiness([FromBody] UpdateBusinessCommand command)
    {
        var result = await _sender.Send(command);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Ok();
    }
}
