using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Admin.Conference.VerifyConference;

[Tags("Admin")]
[Route("api/admin/conferences/{conferenceId:int}/verify")]
[ApiController]
public class VerifyConferenceController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Admin))]
    [HttpPatch]
    public async Task<IActionResult> VerifyConference([FromRoute] int conferenceId, [FromBody] bool isApproved)
    {
        var command = new VerifyConferenceCommand(conferenceId, isApproved);
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}