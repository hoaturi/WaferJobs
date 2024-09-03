using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.InviteBusinessMember.AcceptInvitation;

public record AcceptInvitationCommand(
    string Password,
    string Token,
    string FirstName,
    string LastName,
    string Title
) : IRequest<Result<Unit, Error>>;