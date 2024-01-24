using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Auth")]
[ApiController]
[Route("api/auth/sign-up")]
public class SignUpController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
    {
        var result = await _sender.Send(command);

        if (!result.IsSuccess)
        {
            return this.HandleFailure(result.Error!);
        }

        return Ok();
    }
}
