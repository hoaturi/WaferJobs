using JobBoard.Common.Models;
using JobBoard.Infrastructure.Services.JobMetricService;
using MediatR;

namespace JobBoard.Features.JobMetric.IncrementApplicationCount;

public class IncrementApplicationCountCommandHandler(
    IJobMetricService jobMetricService
) : IRequestHandler<IncrementApplicationCountCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(IncrementApplicationCountCommand command,
        CancellationToken cancellationToken)
    {
        await jobMetricService.IncrementApplicationCountJob(command.Id, cancellationToken);

        return Unit.Value;
    }
}