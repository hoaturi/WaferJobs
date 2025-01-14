﻿using WaferJobs.Infrastructure.Services.JobMetricService;

namespace WaferJobs.Infrastructure.BackgroundJobs.PersistApplicationCountJob;

public class PersistApplicationCountJob(IJobMetricService jobMetricService, ILogger<PersistApplicationCountJob> logger)
    : IRecurringJobBase
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting application count persistence");
        await jobMetricService.PersistApplicationCountAsync(cancellationToken);
        logger.LogInformation("Completed application count persistence");
    }
}