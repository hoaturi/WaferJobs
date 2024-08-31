using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.ClaimBusiness.CompleteBusinessClaim;

public record CompleteBusinessClaimCommand(
    string Token,
    CompleteBusinessClaimRequestDto Dto
) : IRequest<Result<Unit, Error>>;