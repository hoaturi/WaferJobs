using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.InviteBusinessMember.SendInvitation;

public record SendInvitationCommand(string Email) : IRequest<Result<Unit, Error>>;