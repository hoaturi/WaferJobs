namespace JobBoard.Infrastructure.Services.CachingServices.JobPostCountService;

public interface IJobPostCountService
{
    Task<int> GetJobPostCountAsync(CancellationToken cancellationToken);
}