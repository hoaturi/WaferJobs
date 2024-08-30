using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.BusinessClaim.ConfirmBusinessClaim;

public record ConfirmBusinessClaimCommand(int Pin) : IRequest<Result<Unit, Error>>;