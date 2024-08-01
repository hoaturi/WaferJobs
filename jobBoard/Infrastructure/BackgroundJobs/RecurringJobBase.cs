namespace JobBoard.Infrastructure.BackgroundJobs;

public interface IRecurringJobBase
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}