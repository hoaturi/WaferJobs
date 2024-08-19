using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Admin.Business.VerifyBusinessClaim;

public record VerifyBusinessClaimCommand(Guid ClaimId, VerifyBusinessClaimDto Dto) : IRequest<Result<Unit, Error>>;