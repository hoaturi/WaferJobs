using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.VerifyBusinessClaimToken;

public record VerifyBusinessClaimTokenQuery(string Token)
    : IRequest<Result<VerifyBusinessClaimTokenResponse, Error>>;