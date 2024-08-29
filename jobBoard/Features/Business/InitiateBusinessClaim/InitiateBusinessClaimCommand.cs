using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.InitiateBusinessClaim;

public record InitiateBusinessClaimCommand(Guid BusinessId) : IRequest<Result<Unit, Error>>;