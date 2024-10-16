using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Business.ClaimBusiness.CompleteBusinessClaim;

public record CompleteBusinessClaimCommand(
    string Token,
    CompleteBusinessClaimRequestDto Dto
) : IRequest<Result<Unit, Error>>;