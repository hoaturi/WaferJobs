using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobMetric.IncrementApplyCount;

public record IncrementApplyCountCommand(Guid Id) : IRequest<Result<Unit, Error>>;