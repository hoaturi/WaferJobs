using MediatR;
using WaferJobs.Common.Models;
using WaferJobs.Infrastructure.Services.JobMetricService;

namespace WaferJobs.Features.JobMetric.IncrementApplicationCount;

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