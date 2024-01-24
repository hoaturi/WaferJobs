using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Auth")]
[ApiController]
[Route("api/auth/signup/business")]
public class SignUpBusinessController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> SignUpBusiness([FromBody] SignUpBusinessCommand request)
    {
        var result = await _sender.Send(request);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Created();
    }
}
