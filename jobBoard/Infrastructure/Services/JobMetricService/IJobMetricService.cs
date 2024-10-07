namespace JobBoard.Infrastructure.Services.JobMetricService;

public interface IJobMetricService
{
    Task IncrementApplicationCountJob(Guid jobId, CancellationToken cancellationToken);

    Task PersistApplicationCountAsync(CancellationToken cancellationToken);
}