using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.ValidateBusinessCreationToken;

public record ValidateBusinessCreationTokenQuery(string Token)
    : IRequest<Result<ValidateBusinessCreationTokenResponse, Error>>;