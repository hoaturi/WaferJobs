using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobAlert.GetJobAlert;

[Tags("Job Alert")]
[ApiController]
[Route("api/job-alert")]
public class GetJobAlertController(ISender sender) : ControllerBase
{
    [HttpGet("{token}")]
    public async Task<IActionResult> GetJobAlert(string token)
    {
        var result = await sender.Send(new GetJobAlertQuery(token));

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}