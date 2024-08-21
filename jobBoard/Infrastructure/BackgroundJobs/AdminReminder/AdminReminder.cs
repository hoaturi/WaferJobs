using Hangfire;
using JobBoard.Common.Constants;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.EmailService;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.BackgroundJobs.AdminReminder;

public class AdminReminder(
    AppDbContext dbContext,
    IBackgroundJobClient backgroundJobClient,
    ILogger<AdminReminder> logger
) : IRecurringJobBase
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await CheckPendingClaimVerifications(cancellationToken);
        await CheckPendingConferenceSubmissions(cancellationToken);
    }

    private async Task CheckPendingClaimVerifications(CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking for pending business claims");

        var pendingClaims = await dbContext.BusinessClaimAttempts
            .AsNoTracking()
            .Where(bca => bca.Status == ClaimStatus.Pending)
            .Select(bca => new PendingClaimVerificationReminderItem(bca.Business.Name, bca.ExpiresAt!.Value))
            .ToListAsync(cancellationToken);

        logger.LogInformation("Found {Count} pending business claims", pendingClaims.Count);

        if (pendingClaims.Count > 0)
            backgroundJobClient.Enqueue<IEmailService>(x => x.SendPendingClaimVerificationReminderAsync(
                new PendingClaimVerificationReminderDto(pendingClaims)
            ));

        logger.LogInformation("Completed checking for {Count} pending business claims", pendingClaims.Count);
    }

    private async Task CheckPendingConferenceSubmissions(CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking for pending conference submissions");

        var pendingSubmissions = await dbContext.Conferences
            .AsNoTracking()
            .Where(c => !c.IsVerified && !c.IsPublished)
            .Select(c => new PendingConferenceSubmissionReminderItem(c.Title, c.CreatedAt))
            .ToListAsync(cancellationToken);

        logger.LogInformation("Found {Count} pending conference submissions", pendingSubmissions.Count);

        if (pendingSubmissions.Count > 0)
            backgroundJobClient.Enqueue<IEmailService>(x => x.SendPendingConferenceSubmissionReminderAsync(
                new PendingConferenceSubmissionReminderDto(pendingSubmissions)
            ));

        logger.LogInformation("Completed checking for {Count} pending conference submissions",
            pendingSubmissions.Count);
    }
}