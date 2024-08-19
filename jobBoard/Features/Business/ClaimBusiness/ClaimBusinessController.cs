using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.ClaimBusiness;

[Tags("Business")]
[Route("api/businesses/claim")]
[ApiController]
public class ClaimBusinessController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPost]
    public async Task<IActionResult> ClaimBusiness([FromBody] ClaimBusinessCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}