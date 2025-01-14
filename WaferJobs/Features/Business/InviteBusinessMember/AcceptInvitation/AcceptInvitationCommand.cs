﻿using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Business.InviteBusinessMember.AcceptInvitation;

public record AcceptInvitationCommand(
    string Password,
    string Token,
    string FirstName,
    string LastName,
    string Title
) : IRequest<Result<Unit, Error>>;