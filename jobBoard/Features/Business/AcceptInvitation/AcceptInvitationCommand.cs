using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.AcceptInvitation;

public record AcceptInvitationCommand(
    string Token,
    AcceptInvitationDto Dto
) : IRequest<Result<Unit, Error>>;