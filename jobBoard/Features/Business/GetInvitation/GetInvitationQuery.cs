using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.GetInvitation;

public record GetInvitationQuery(string Token) : IRequest<Result<GetInvitationResponse, Error>>;