using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.JobAlert.UpdateJobAlert;

[Tags("Job Alert")]
[ApiController]
[Route("api/job-alerts")]
public class UpdateJobAlertController(ISender sender) : ControllerBase
{
    [HttpPut("{token}")]
    public async Task<IActionResult> UpdateJobAlert([FromRoute] string token, [FromBody] UpdateJobAlertDto dto)
    {
        var result = await sender.Send(new UpdateJobAlertCommand(
            token,
            dto
        ));

        return result.IsSuccess
            ? Ok()
            : BadRequest(result.Error);
    }
}