using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Auth.Signup;

[Tags("Auth")]
[ApiController]
[Route("api/auth/signup")]
public class SignUpController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
    {
        var result = await sender.Send(command);

        return !result.IsSuccess ? this.HandleError(result.Error!) : Ok();
    }
}