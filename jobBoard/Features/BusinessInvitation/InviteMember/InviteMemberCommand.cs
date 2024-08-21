using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.BusinessInvitation.InviteMember;

public record InviteMemberCommand(string InviteeEmail) : IRequest<Result<Unit, Error>>;