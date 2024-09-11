using JobBoard.Common.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.ValidateBusinessCreationToken;

[Tags("Business")]
[Route("api/businesses/creation/validate-token")]
[ApiController]
public class ValidateBusinessCreationTokenController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPost]
    public async Task<IActionResult> ValidateBusinessCreationToken([FromBody] ValidateBusinessCreationTokenQuery query)
    {
        var result = await sender.Send(query);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}