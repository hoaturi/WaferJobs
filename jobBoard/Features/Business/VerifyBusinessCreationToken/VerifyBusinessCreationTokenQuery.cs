using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.VerifyBusinessCreationToken;

public record VerifyBusinessCreationTokenQuery(string Token)
    : IRequest<Result<VerifyBusinessCreationTokenResponse, Error>>;