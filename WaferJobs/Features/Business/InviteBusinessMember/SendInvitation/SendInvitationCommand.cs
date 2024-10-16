using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Business.InviteBusinessMember.SendInvitation;

public record SendInvitationCommand(string Email) : IRequest<Result<Unit, Error>>;