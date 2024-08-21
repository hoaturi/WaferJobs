using JobBoard.Common.Constants;
using JobBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.BackgroundJobs.BusinessExpirationChecker;

public class BusinessExpirationChecker(
    AppDbContext dbContext,
    ILogger<BusinessExpirationChecker> logger)
    : IRecurringJobBase
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await CheckExpiredBusinessClaims(cancellationToken);
        await CheckExpiredBusinessInvitations(cancellationToken);
    }

    private async Task CheckExpiredBusinessClaims(CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking for expired business claims");

        var expiredClaimCount = await dbContext.BusinessClaimAttempts
            .Where(bca => bca.ExpiresAt < DateTime.UtcNow)
            .ExecuteUpdateAsync(
                s => s.SetProperty(bca => bca.Status, ClaimStatus.Expired),
                cancellationToken);

        logger.LogInformation("Completed checking for expired business claims. {ExpiredClaimCount} claims deactivated",
            expiredClaimCount);
    }

    private async Task CheckExpiredBusinessInvitations(CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking for expired business invitations");

        var expiredInvitationCount = await dbContext.BusinessMemberInvitations
            .Where(bmi => bmi.ExpiresAt < DateTime.UtcNow)
            .ExecuteUpdateAsync(s =>
                    s.SetProperty(bmi => bmi.IsActive, false),
                cancellationToken);

        logger.LogInformation(
            "Completed checking for expired business invitations. {ExpiredInvitationCount} invitations deactivated",
            expiredInvitationCount);
    }
}