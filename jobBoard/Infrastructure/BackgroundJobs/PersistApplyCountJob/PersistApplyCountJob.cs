using JobBoard.Infrastructure.Services.JobMetricService;

namespace JobBoard.Infrastructure.BackgroundJobs.PersistApplyCountJob;

public class PersistApplyCountJob(IJobMetricService jobMetricService) : IRecurringJobBase
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await jobMetricService.PersistApplyCountAsync(cancellationToken);
    }
}