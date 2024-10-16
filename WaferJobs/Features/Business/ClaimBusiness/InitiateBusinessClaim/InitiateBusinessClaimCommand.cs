using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Business.ClaimBusiness.InitiateBusinessClaim;

public record InitiateBusinessClaimCommand(Guid BusinessId) : IRequest<Result<Unit, Error>>;