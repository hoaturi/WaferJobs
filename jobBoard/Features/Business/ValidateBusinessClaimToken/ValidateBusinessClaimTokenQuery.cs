using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.ValidateBusinessClaimToken;

public record ValidateBusinessClaimTokenQuery(string Token)
    : IRequest<Result<ValidateBusinessClaimTokenResponse, Error>>;