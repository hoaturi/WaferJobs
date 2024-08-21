using Hangfire;
using JobBoard.Common.Constants;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.EmailService;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.BackgroundJobs.BusinessChecker;

public class BusinessChecker(
    AppDbContext dbContext,
    IBackgroundJobClient backgroundJobClient,
    ILogger<BusinessChecker> logger)
    : IRecurringJobBase
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await CheckPendingClaimVerifications(cancellationToken);
        await CheckExpiredBusinessClaims(cancellationToken);
        await CheckExpiredBusinessInvitations(cancellationToken);
    }

    private async Task CheckPendingClaimVerifications(CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking for pending business claim verification attempts");

        var pendingClaims = await dbContext.BusinessClaimAttempts
            .AsNoTracking()
            .Where(bca => bca.Status == ClaimStatus.Pending)
            .Select(bca => new PendingClaimVerificationReminderItem(bca.Business.Name, bca.ExpiresAt!.Value))
            .ToListAsync(cancellationToken);

        if (pendingClaims.Count > 0)
            backgroundJobClient.Enqueue<EmailService>(x => x.SendPendingClaimVerificationReminderAsync(
                new PendingClaimVerificationReminderDto(pendingClaims)
            ));

        logger.LogInformation("Completed checking for pending business claim verification attempts");
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