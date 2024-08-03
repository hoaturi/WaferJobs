using Hangfire;
using JobBoard.Common.Middlewares;
using JobBoard.Infrastructure.BackgroundJobs.FeaturedJobExpirationChecker;
using JobBoard.Infrastructure.BackgroundJobs.JobAlertSender;
using JobBoard.Infrastructure.BackgroundJobs.LookupDataCacheUpdater;

namespace JobBoard.Infrastructure.Extensions;

public static class ApplicationExtensions
{
    public static IApplicationBuilder UseHangfireJobs(this IApplicationBuilder app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            StatsPollingInterval = 60000
        });

        RecurringJob.AddOrUpdate<JobAlertSender>(
            "JobAlertSender",
            x => x.ExecuteAsync(CancellationToken.None),
            "0 */6 * * *"
        );

        RecurringJob.AddOrUpdate<FeaturedJobExpirationChecker>(
            "FeaturedJobExpirationChecker",
            x => x.ExecuteAsync(CancellationToken.None),
            Cron.Hourly
        );

        RecurringJob.AddOrUpdate<LookupDataCacheUpdater>(
            "LookupDataCacheUpdater",
            x => x.ExecuteAsync(CancellationToken.None),
            Cron.Hourly
        );

        return app;
    }


    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }
}