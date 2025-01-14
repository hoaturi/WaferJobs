﻿using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Domain.Auth;

namespace WaferJobs.Features.Auth.Refresh;

[Tags("Auth")]
[ApiController]
[Route("api/auth/refresh")]
public class RefreshController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Refresh()
    {
        HttpContext.Request.Cookies.TryGetValue("refresh_token", out var refreshToken);

        if (refreshToken is null) return this.HandleError(AuthErrors.InvalidRefreshToken);

        var result = await sender.Send(new RefreshCommand(refreshToken));

        return result.IsSuccess
            ? Ok(result.Value)
            : this.HandleError(result.Error);
    }
}