using JobBoard.Common.Constants;
using JobBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.BackgroundJobs.BusinessClaimExpirationChecker;

public class BusinessClaimExpirationChecker(
    AppDbContext dbContext,
    ILogger<BusinessClaimExpirationChecker> logger)
    : IRecurringJobBase
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking for expired business claims");

        var expiredClaimCount = await dbContext.BusinessClaimAttempts
            .Where(bca => bca.ExpiresAt < DateTime.UtcNow)
            .ExecuteUpdateAsync(
                s =>
                    s.SetProperty(bca => bca.Status, ClaimStatus.Expired), cancellationToken);

        logger.LogInformation("Completed checking for expired business claims. {ExpiredClaimCount} claims deactivated",
            expiredClaimCount);
    }
}