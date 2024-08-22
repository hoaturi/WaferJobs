using JobBoard.Common.Constants;
using JobBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard.Infrastructure.Services.CachingServices.JobPostCountService;

public class JobPostCountService(AppDbContext dbContext, IDistributedCache cache, ILogger<JobPostCountService> logger)
    : IJobPostCountService
{
    public async Task<int> GetJobPostCountAsync(CancellationToken cancellationToken)
    {
        var totalCount = await cache.GetStringAsync(CacheKeys.JobCount, cancellationToken);

        if (totalCount is not null) return int.Parse(totalCount);

        var count = await dbContext.JobPosts.Where(
                jp => !jp.IsDeleted && jp.IsPublished
            )
            .CountAsync(cancellationToken);

        await cache.SetStringAsync(CacheKeys.JobCount, count.ToString(), cancellationToken);
        logger.LogInformation("Updated job post count cache with {Count} job posts", count);

        return count;
    }
}