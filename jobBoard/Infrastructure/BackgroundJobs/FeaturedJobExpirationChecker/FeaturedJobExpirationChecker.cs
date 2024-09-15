using JobBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.BackgroundJobs.FeaturedJobExpirationChecker;

public class FeaturedJobExpirationChecker(AppDbContext dbContext, ILogger<FeaturedJobExpirationChecker> logger)
    : IRecurringJobBase
{
    private const int ExpirationDays = 35;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Featured Job Expiration Check");

        var expirationDate = DateTime.UtcNow.AddDays(-ExpirationDays);

        var updatedCount = await dbContext.JobPosts
            .Where(jp => jp.IsFeatured && jp.IsPublished && jp.PublishedAt != null &&
                         jp.PublishedAt < expirationDate)
            .ExecuteUpdateAsync(s =>
                    s.SetProperty(jp => jp.IsFeatured, false)
                , cancellationToken);

        logger.LogInformation(
            "Completed Featured Job Expiration Check. Downgraded {JobPostCount} featured jobs to regular jobs.",
            updatedCount);
    }
}