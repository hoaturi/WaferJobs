using Microsoft.EntityFrameworkCore;
using WaferJobs.Infrastructure.Persistence;

namespace WaferJobs.Infrastructure.BackgroundJobs.FeaturedJobStatusUpdateJob;

public class FeaturedJobStatusUpdateJob(AppDbContext dbContext, ILogger<FeaturedJobStatusUpdateJob> logger)
    : IRecurringJobBase
{
    private const int ExpirationDays = 35;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting featured job posts status update");

        var expirationDate = DateTime.UtcNow.AddDays(-ExpirationDays);

        var updatedCount = await dbContext.JobPosts
            .Where(jp => jp.IsFeatured && jp.IsPublished && jp.PublishedAt != null &&
                         jp.PublishedAt < expirationDate)
            .ExecuteUpdateAsync(s =>
                    s.SetProperty(jp => jp.IsFeatured, false)
                , cancellationToken);

        logger.LogInformation(
            "Completed featured job posts status update. Reverted {UpdatedCount} posts to regular status",
            updatedCount);
    }
}