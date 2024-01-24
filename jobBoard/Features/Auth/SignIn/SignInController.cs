using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Auth")]
[ApiController]
[Route("api/auth/signin")]
public class SignInController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> SignIn([FromBody] SignInCommand command)
    {
        var result = await _sender.Send(command);

        if (!result.IsSuccess)
        {
            return this.HandleFailure(result.Error!);
        }

        return Ok(result.Value);
    }
}
