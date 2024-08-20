using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.InviteMember;

public record InviteMemberCommand(string InviteeEmail) : IRequest<Result<Unit, Error>>;