using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.JobAlert.UpdateJobAlert;

public record UpdateJobAlertCommand(
    string Token,
    UpdateJobAlertDto Dto
) : IRequest<Result<Unit, Error>>;