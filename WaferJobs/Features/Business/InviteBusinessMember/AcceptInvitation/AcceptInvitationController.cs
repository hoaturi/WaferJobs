using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WaferJobs.Features.Business.InviteBusinessMember.AcceptInvitation;

[Tags("Business")]
[Route("/api/businesses/invitations/accept")]
[ApiController]
public class AcceptInvitationController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitationCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}