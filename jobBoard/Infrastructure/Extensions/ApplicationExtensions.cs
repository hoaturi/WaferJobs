using Hangfire;
using JobBoard.Common.Middlewares;
using JobBoard.Infrastructure.BackgroundJobs.CurrencyRateRefreshJob;
using JobBoard.Infrastructure.BackgroundJobs.FeaturedJobStatusUpdateJob;
using JobBoard.Infrastructure.BackgroundJobs.JobAlertEmailSenderJob;
using JobBoard.Infrastructure.BackgroundJobs.PersistApplicationCountJob;

namespace JobBoard.Infrastructure.Extensions;

public static class ApplicationExtensions
{
    public static void UseHangfireJobs(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            StatsPollingInterval = 60000
        });

        RecurringJob.AddOrUpdate<JobAlertEmailSenderJob>(
            "JobAlertEmailSenderJob",
            x => x.ExecuteAsync(CancellationToken.None),
            "0 */6 * * *"
        );

        RecurringJob.AddOrUpdate<FeaturedJobStatusUpdateJob>(
            "FeaturedJobStatusUpdateJob",
            x => x.ExecuteAsync(CancellationToken.None),
            Cron.Hourly
        );

        RecurringJob.AddOrUpdate<CurrencyRateRefreshJob>(
            "CurrencyRateRefreshJob",
            x => x.ExecuteAsync(CancellationToken.None),
            Cron.Daily
        );

        RecurringJob.AddOrUpdate<PersistApplicationCountJob>(
            "PersistApplicationCountJob",
            x => x.ExecuteAsync(CancellationToken.None),
            "*/30 * * * *"
        );
    }

    public static void UseMiddleWares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}