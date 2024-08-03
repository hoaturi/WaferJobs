namespace JobBoard.Infrastructure.Services.LookupServices.JobPostCountService;

public interface IJobPostCountService
{
    Task<int> GetJobPostCountAsync(CancellationToken cancellationToken);
}