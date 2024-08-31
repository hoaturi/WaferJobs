using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.ClaimBusiness.InitiateBusinessClaim;

public record InitiateBusinessClaimCommand(Guid BusinessId) : IRequest<Result<Unit, Error>>;