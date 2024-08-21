using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.BusinessInvitation.GetInvitation;

public record GetInvitationQuery(string Token) : IRequest<Result<GetInvitationResponse, Error>>;