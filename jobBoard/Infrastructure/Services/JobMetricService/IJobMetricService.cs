namespace JobBoard.Infrastructure.Services.JobMetricService;

public interface IJobMetricService
{
    Task IncrementApplyCountAsync(Guid jobId, CancellationToken cancellationToken);

    Task PersistApplyCountAsync(CancellationToken cancellationToken);
}