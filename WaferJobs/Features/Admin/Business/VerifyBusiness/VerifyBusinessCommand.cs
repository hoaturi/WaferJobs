using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Admin.Business.VerifyBusiness;

public record VerifyBusinessCommand(Guid BusinessId, bool IsApproved) : IRequest<Result<Unit, Error>>;