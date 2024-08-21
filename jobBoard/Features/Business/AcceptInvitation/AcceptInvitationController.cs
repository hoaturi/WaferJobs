using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.AcceptInvitation;

[Tags("Business")]
[Route("api/businesses/invitations/accept")]
[ApiController]
public class AcceptInvitationController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AcceptBusinessInvitation([FromQuery] string token,
        [FromBody] AcceptInvitationDto dto)
    {
        var result = await sender.Send(new AcceptInvitationCommand(token, dto));

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}