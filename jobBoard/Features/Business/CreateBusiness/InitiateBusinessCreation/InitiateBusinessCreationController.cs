﻿using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.Business.CreateBusiness.InitiateBusinessCreation;

[Tags("Business")]
[Route("api/businesses")]
[ApiController]
public class InitiateBusinessCreationController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPost]
    public async Task<IActionResult> InitiateBusinessCreation([FromBody] InitiateBusinessCreationCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}