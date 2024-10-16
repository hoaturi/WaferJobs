using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WaferJobs.Features.Auth.Signup;

[Tags("Auth")]
[ApiController]
[Route("api/auth/signup")]
public class SignUpController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess ? Ok() : this.HandleError(result.Error!);
    }
}