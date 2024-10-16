using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.JobMetric.IncrementApplicationCount;

public record IncrementApplicationCountCommand(Guid Id) : IRequest<Result<Unit, Error>>;