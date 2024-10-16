using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Business.VerifyBusinessClaimToken;

public record VerifyBusinessClaimTokenQuery(string Token)
    : IRequest<Result<VerifyBusinessClaimTokenResponse, Error>>;