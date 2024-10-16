using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.JobAlert.GetJobAlert;

public record GetJobAlertQuery(string Token) : IRequest<Result<GetJobAlertResponse, Error>>;