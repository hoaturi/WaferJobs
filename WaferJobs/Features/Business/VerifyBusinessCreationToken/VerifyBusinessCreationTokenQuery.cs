using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Business.VerifyBusinessCreationToken;

public record VerifyBusinessCreationTokenQuery(string Token)
    : IRequest<Result<VerifyBusinessCreationTokenResponse, Error>>;