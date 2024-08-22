using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Admin.Conference.GetPendingConferenceSubmissions;

[Tags("Admin")]
[Route("api/admin/conferences/pending")]
[ApiController]
public class GetPendingConferenceSubmissionsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPendingConferenceSubmissions()
    {
        var result = await sender.Send(new GetPendingConferenceSubmissionsQuery());

        return result.IsSuccess
            ? Ok(result.Value)
            : this.HandleError(result.Error);
    }
}