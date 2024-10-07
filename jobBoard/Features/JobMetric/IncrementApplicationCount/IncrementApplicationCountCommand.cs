using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobMetric.IncrementApplicationCount;

public record IncrementApplicationCountCommand(Guid Id) : IRequest<Result<Unit, Error>>;