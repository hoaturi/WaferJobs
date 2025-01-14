﻿using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WaferJobs.Features.JobAlert.GetJobAlert;

[Tags("Job Alert")]
[ApiController]
[Route("api/job-alerts")]
public class GetJobAlertController(ISender sender) : ControllerBase
{
    [HttpGet("{token}")]
    public async Task<IActionResult> GetJobAlert(string token)
    {
        var result = await sender.Send(new GetJobAlertQuery(token));

        return result.IsSuccess ? Ok(result.Value) : this.HandleError(result.Error);
    }
}