﻿using JobBoard.Common.Constants;
using JobBoard.Common.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobBoard.Features.BusinessInvitation.InviteMember;

[Tags("Business")]
[Route("api/businesses/invitations")]
[ApiController]
public class InviteMemberController(ISender sender) : ControllerBase
{
    [Authorize(nameof(UserRoles.Business))]
    [HttpPost]
    public async Task<IActionResult> InviteMember([FromBody] InviteMemberCommand command)
    {
        var result = await sender.Send(command);

        return result.IsSuccess
            ? Ok()
            : this.HandleError(result.Error);
    }
}