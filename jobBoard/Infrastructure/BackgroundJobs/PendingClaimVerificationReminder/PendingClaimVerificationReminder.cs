using Hangfire;
using JobBoard.Common.Constants;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.EmailService;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.BackgroundJobs.PendingClaimVerificationReminder;

public class PendingClaimVerificationReminder(
    AppDbContext dbContext,
    IBackgroundJobClient backgroundJobClient,
    ILogger<PendingClaimVerificationReminder> logger
) : IRecurringJobBase
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
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
}