﻿using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WaferJobs.Features.Auth.ConfirmEmail;

[Tags("Auth")]
[Route("api/auth")]
[ApiController]
public class ConfirmEmailController(ISender sender) : ControllerBase
{
    [HttpPatch("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        var result = await sender.Send(new ConfirmEmailCommand(userId, token));

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}