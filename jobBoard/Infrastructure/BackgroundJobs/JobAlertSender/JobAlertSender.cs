using System.Text;
using JobBoard.Common.Extensions;
using JobBoard.Domain.JobAlert;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.EmailService;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Infrastructure.BackgroundJobs.JobAlertSender;

public class JobAlertSender(AppDbContext dbContext, IEmailService emailService, ILogger<JobAlertSender> logger)
    : IRecurringJobBase
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Send Job Alerts Job execution");

        var jobAlerts = await GetJobAlertsToSend(cancellationToken);

        foreach (var jobAlert in jobAlerts)
        {
            var jobPostsToSend = await GetJobPostsToSend(jobAlert, cancellationToken);

            if (jobPostsToSend?.TotalCount > 0)
            {
                var jobAlertEmailDto = new JobAlertEmailDto(
                    jobAlert.Email,
                    jobAlert.Keyword,
                    jobAlert.Country?.Label,
                    jobAlert.Categories.Select(c => c.Label).ToList(),
                    jobAlert.EmploymentTypes.Select(et => et.Label).ToList(),
                    jobAlert.Token,
                    GenerateFilterQueryString(jobAlert),
                    jobAlert.LastSentAt ?? jobAlert.CreatedAt,
                    jobPostsToSend
                );


                await emailService.SendJobAlertAsync(jobAlertEmailDto);
            }

            jobAlert.LastSentAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
        }


        logger.LogInformation("Completed Send Job Alerts Job execution. Processed {JobAlertCount} alerts",
            jobAlerts.Count);
    }

    private async Task<JobPostsWithCountDto> GetJobPostsToSend(JobAlertEntity jobAlert,
        CancellationToken cancellationToken)
    {
        var referenceDate = jobAlert.LastSentAt ?? jobAlert.CreatedAt;

        var query = dbContext.JobPosts
            .Where(jp => jp.IsPublished && !jp.IsDeleted)
            .Where(jp => jp.PublishedAt != null && jp.PublishedAt > referenceDate);

        if (!string.IsNullOrEmpty(jobAlert.Keyword))
            query = query.Where(jp => jp.SearchVector.Matches(jobAlert.Keyword));

        if (jobAlert.CountryId != null) query = query.Where(jp => jp.CountryId == jobAlert.CountryId);

        var categoryIds = jobAlert.Categories.Select(c => c.Id).ToList();
        var employmentTypeIds = jobAlert.EmploymentTypes.Select(et => et.Id).ToList();


        if (categoryIds.Count != 0) query = query.Where(jp => categoryIds.Contains(jp.CategoryId));

        if (employmentTypeIds.Count != 0) query = query.Where(jp => employmentTypeIds.Contains(jp.EmploymentTypeId));

        var totalCount = await query.CountAsync(cancellationToken);

        var jobPosts = await query
            .OrderByDescending(jp => jp.IsFeatured)
            .ThenByDescending(jp => jp.PublishedAt)
            .ThenByDescending(jp =>
                jobAlert.Keyword != null ? jp.SearchVector.Rank(EF.Functions.ToTsQuery(jobAlert.Keyword)) : 0)
            .Select(jp => new JobPostToSendDto(
                jp.Id,
                jp.Title,
                jp.City != null ? $"{jp.City.Label}, {jp.Country.Label}" : jp.Country.Label,
                jp.EmploymentType.Label,
                jp.CompanyName,
                jp.CompanyLogoUrl,
                jp.PublishedAt!.Value.ToRelativeTimeString(),
                jp.IsRemote,
                jp.IsFeatured
            ))
            .Take(15)
            .ToListAsync(cancellationToken);

        return new JobPostsWithCountDto(jobPosts, totalCount);
    }

    private async Task<List<JobAlertEntity>> GetJobAlertsToSend(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var weekAgo = now.AddDays(-7);

        return await dbContext.JobAlerts
            .Include(ja => ja.EmploymentTypes)
            .Include(ja => ja.Categories)
            .Where(ja =>
                (ja.LastSentAt == null && ja.CreatedAt <= now) ||
                (ja.LastSentAt != null && ja.LastSentAt <= weekAgo)
            )
            .ToListAsync(cancellationToken);
    }

    private static string GenerateFilterQueryString(JobAlertEntity jobAlert)
    {
        var query = new StringBuilder();

        if (!string.IsNullOrEmpty(jobAlert.Keyword)) query.Append($"keyword={jobAlert.Keyword.ToLower().Trim()}&");
        if (jobAlert.Country != null) query.Append($"country={jobAlert.Country.Slug}&");
        if (jobAlert.Categories.Count != 0)
            query.Append($"categories={string.Join(",", jobAlert.Categories.Select(c => c.Slug))}&");
        if (jobAlert.EmploymentTypes.Count != 0)
            query.Append($"employmentTypes={string.Join(",", jobAlert.EmploymentTypes.Select(et => et.Slug))}&");

        return query.ToString();
    }
}