using JobBoard.Common.Models;
using JobBoard.Domain.JobPost;
using JobBoard.Features.JobPost.GetJobPost;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.LookupServices.CurrencyService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.GetJobPostList;

public class GetJobPostListQueryHandler(AppDbContext dbContext, ICurrencyService currencyService)
    : IRequestHandler<GetJobPostListQuery, Result<GetJobPostListResponse, Error>>
{
    public async Task<Result<GetJobPostListResponse, Error>> Handle(GetJobPostListQuery query,
        CancellationToken cancellationToken)
    {
        var jobPostListQuery = BuildJobPostListQuery(query);

        if (query.Currency is not null && (query.MinSalary is not null || query.MaxSalary is not null))
            jobPostListQuery = await ApplyCurrencyFilterAsync(query, jobPostListQuery, cancellationToken);

        var totalJobPostCount = await jobPostListQuery.CountAsync(cancellationToken);

        var jobPostList = await jobPostListQuery
            .OrderByDescending(j => j.IsFeatured)
            .ThenByDescending(j => j.PublishedAt)
            .Skip((query.Page - 1) * query.Take)
            .Take(query.Take)
            .Select(j =>
                new GetJobPostResponse(
                    j.Id,
                    j.Category.Label,
                    j.Country.Label,
                    j.EmploymentType.Label,
                    j.Title,
                    j.Description,
                    j.IsRemote,
                    j.IsFeatured,
                    j.CompanyName,
                    j.ExperienceLevel != null ? j.ExperienceLevel.Label : null,
                    j.City != null ? j.City.Label : null,
                    j.MinSalary,
                    j.MaxSalary,
                    j.Currency != null ? j.Currency.Code : null,
                    null,
                    j.BusinessId,
                    j.CompanyLogoUrl,
                    j.CompanyWebsiteUrl,
                    j.Tags.Select(t => t.Label).ToList(),
                    j.FeaturedStartDate.GetValueOrDefault(),
                    j.FeaturedEndDate.GetValueOrDefault(),
                    j.PublishedAt.GetValueOrDefault()
                ))
            .ToListAsync(cancellationToken);

        return new GetJobPostListResponse(jobPostList, totalJobPostCount);
    }

    private IQueryable<JobPostEntity> BuildJobPostListQuery(GetJobPostListQuery query)
    {
        var jobPostListQuery = dbContext.JobPosts
            .AsNoTracking()
            .Where(j => j.IsPublished && !j.IsDeleted);

        if (query.Keyword is not null)
            jobPostListQuery = jobPostListQuery.Where(jp => jp.SearchVector.Matches(query.Keyword.ToLower()));

        if (query.City is not null)
            jobPostListQuery = jobPostListQuery.Where(j => j.City != null && j.City.Id == query.City);

        if (query.Country is not null)
            jobPostListQuery = jobPostListQuery.Where(j => j.Country.Id == query.Country);

        if (query.ExperienceLevel is not null)
            jobPostListQuery = jobPostListQuery.Where(j =>
                j.ExperienceLevel != null && j.ExperienceLevel.Id == query.ExperienceLevel);

        if (query.RemoteOnly is true)
            jobPostListQuery = jobPostListQuery.Where(j => j.IsRemote);

        if (query.Categories?.Count > 0)
            jobPostListQuery = jobPostListQuery.Where(j => query.Categories.Contains(j.Category.Id));

        if (query.EmploymentTypes?.Count > 0)
            jobPostListQuery = jobPostListQuery.Where(j => query.EmploymentTypes.Contains(j.EmploymentType.Id));

        if (query.PostedDate is not null)
        {
            var postedDate = DateTime.UtcNow.AddDays(-query.PostedDate.Value);
            jobPostListQuery = jobPostListQuery.Where(j => j.PublishedAt >= postedDate);
        }

        if (query.FeaturedOnly is true)
            jobPostListQuery = jobPostListQuery.Where(j => j.IsFeatured);

        return jobPostListQuery;
    }

    private async Task<IQueryable<JobPostEntity>> ApplyCurrencyFilterAsync(
        GetJobPostListQuery query,
        IQueryable<JobPostEntity> jobPostListQuery,
        CancellationToken cancellationToken)
    {
        var currencies = await currencyService.GetExchangeRatesAsync(cancellationToken);
        var targetCurrency = currencies.FirstOrDefault(c => c.Id == query.Currency);

        if (targetCurrency is null) return jobPostListQuery;

        if (query.MinSalary is not null)
        {
            var minSalaryInUsd = query.MinSalary.Value / targetCurrency.Rate;
            jobPostListQuery = jobPostListQuery.Where(j =>
                j.Currency != null && j.MinSalary != null &&
                j.MinSalary.Value / j.Currency.Rate >= minSalaryInUsd);
        }

        if (query.MaxSalary is null) return jobPostListQuery;

        var maxSalaryInUsd = query.MaxSalary.Value / targetCurrency.Rate;
        jobPostListQuery = jobPostListQuery.Where(j =>
            j.Currency != null && j.MaxSalary != null &&
            j.MaxSalary.Value / j.Currency.Rate <= maxSalaryInUsd);

        return jobPostListQuery;
    }
}