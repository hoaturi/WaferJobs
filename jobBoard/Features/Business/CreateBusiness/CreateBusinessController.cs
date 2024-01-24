using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard;

[Tags("Business")]
[ApiController]
[Route("api/businesses")]
public class CreateBusinessController(ISender sender) : BaseController
{
    private readonly ISender _sender = sender;

    [Authorize(policy: RolePolicy.Business)]
    [HttpPost]
    public async Task<IActionResult> CreateBusiness(CreateBusinessCommand command)
    {
        var result = await _sender.Send(command);

        if (!result.IsSuccess)
        {
            return HandleFailure(result.Error);
        }

        return Created();
    }
}
