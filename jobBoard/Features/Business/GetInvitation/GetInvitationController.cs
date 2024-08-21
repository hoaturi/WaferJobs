using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.GetInvitation;

[Tags("Business")]
[Route("api/businesses/invitations")]
[ApiController]
public class GetInvitationController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetInvitation([FromQuery] string token)
    {
        var result = await sender.Send(new GetInvitationQuery(token));

        return result.IsSuccess
            ? Ok(result.Value)
            : this.HandleError(result.Error);
    }
}