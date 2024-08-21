using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Conference;

[Tags("Conference")]
[Route("api/conferences")]
[ApiController]
public class SubmitConferenceController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SubmitConference([FromBody] SubmitConferenceCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}