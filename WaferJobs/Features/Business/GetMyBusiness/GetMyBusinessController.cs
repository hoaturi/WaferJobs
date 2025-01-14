﻿using WaferJobs.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaferJobs.Common.Constants;

namespace WaferJobs.Features.Business.GetMyBusiness;

[Tags("Business")]
[ApiController]
[Route("api/businesses")]
public class GetMyBusinessController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyBusiness()
    {
        var result = await sender.Send(new GetMyBusinessQuery());

        return !result.IsSuccess ? this.HandleError(result.Error) : Ok(result.Value);
    }
}