using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Admin.Business.VerifyBusiness;

public record VerifyBusinessCommand(Guid BusinessId, bool IsApproved) : IRequest<Result<Unit, Error>>;