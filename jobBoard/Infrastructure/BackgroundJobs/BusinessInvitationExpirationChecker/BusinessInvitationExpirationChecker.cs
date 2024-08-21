using JobBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.BackgroundJobs.BusinessInvitationExpirationChecker;

public class BusinessInvitationExpirationChecker(
    AppDbContext dbContext,
    ILogger<BusinessInvitationExpirationChecker> logger) : IRecurringJobBase
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking for expired business invitations");

        var expiredInvitationCount = await dbContext.BusinessMemberInvitations
            .Where(bmi => bmi.ExpiresAt < DateTime.UtcNow)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(bmi => bmi.IsActive, false), cancellationToken);

        logger.LogInformation(
            "Completed checking for expired business invitations. {ExpiredInvitationCount} invitations deactivated",
            expiredInvitationCount);
    }
}