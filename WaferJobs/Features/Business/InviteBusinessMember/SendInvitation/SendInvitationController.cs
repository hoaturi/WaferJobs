using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WaferJobs.Features.Business.InviteBusinessMember.SendInvitation;

[Tags("Business")]
[Route("api/businesses/invitations")]
[ApiController]
public class SendInvitationController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SendInvitation([FromBody] SendInvitationCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}