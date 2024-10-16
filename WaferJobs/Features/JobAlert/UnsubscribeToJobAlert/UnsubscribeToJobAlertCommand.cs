using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.JobAlert.UnsubscribeToJobAlert;

public record UnsubscribeToJobAlertCommand(string Token) : IRequest<Result<Unit, Error>>;