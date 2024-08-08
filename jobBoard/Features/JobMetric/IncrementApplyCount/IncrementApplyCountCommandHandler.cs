using JobBoard.Common.Models;
using JobBoard.Infrastructure.Services.JobMetricService;
using MediatR;

namespace JobBoard.Features.JobMetric.IncrementApplyCount;

public class IncrementApplyCountCommandHandler(
    IJobMetricService jobMetricService
) : IRequestHandler<IncrementApplyCountCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(IncrementApplyCountCommand command,
        CancellationToken cancellationToken)
    {
        await jobMetricService.IncrementApplyCountAsync(command.Id, cancellationToken);

        return Unit.Value;
    }
}