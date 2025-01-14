﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Models;
using WaferJobs.Domain.JobPost;
using WaferJobs.Infrastructure.Persistence;

namespace WaferJobs.Features.JobPost.GetJobPosts;

public class GetJobPostsQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetJobPostsQuery, Result<GetJobPostsResponse, Error>>
{
    public async Task<Result<GetJobPostsResponse, Error>> Handle(GetJobPostsQuery query,
        CancellationToken cancellationToken)
    {
        var jobPostsQuery = BuildJobPostListQuery(query);

        var jobPostCount = await jobPostsQuery.CountAsync(cancellationToken);

        var jobPosts = await jobPostsQuery
            .OrderByDescending(j => j.IsFeatured)
            .ThenByDescending(j => j.PublishedAt)
            .Skip((query.Page - 1) * query.Take)
            .Take(query.Take)
            .Select(j => new JobPostDto(
                new JobDetails(
                    j.Id,
                    j.Category.Label,
                    j.Country.Label,
                    j.EmploymentType.Label,
                    j.Title,
                    j.Description,
                    j.IsRemote,
                    j.IsFeatured,
                    j.Slug,
                    j.ExperienceLevel != null ? j.ExperienceLevel.Label : null,
                    j.City != null ? j.City.Label : null,
                    j.MinSalary,
                    j.MaxSalary,
                    j.Currency != null ? j.Currency.Code : null,
                    j.Tags.Select(t => t.Label).ToList(),
                    j.FeaturedStartDate.GetValueOrDefault(),
                    j.FeaturedEndDate.GetValueOrDefault(),
                    j.PublishedAt.GetValueOrDefault()
                ),
                new BusinessDetails(
                    j.BusinessId,
                    j.CompanyName,
                    j.CompanyLogoUrl,
                    j.CompanyWebsiteUrl
                )
            ))
            .ToListAsync(cancellationToken);

        return new GetJobPostsResponse(jobPosts, jobPostCount);
    }

    private IQueryable<JobPostEntity> BuildJobPostListQuery(GetJobPostsQuery query)
    {
        var jobPostsQuery = dbContext.JobPosts
            .AsNoTracking()
            .Where(j => j.IsPublished);

        if (query.Keyword is not null)
        {
            var keywords = query.Keyword.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            jobPostsQuery = jobPostsQuery.Where(jp =>
                keywords.All(keyword =>
                    jp.SearchVector.Matches(keyword) ||
                    jp.Tags.Any(t => t.Slug.Contains(keyword))));
        }

        if (query.City is not null)
            jobPostsQuery = jobPostsQuery.Where(j => j.City != null && j.City.Id == query.City);

        if (query.Country is not null)
            jobPostsQuery = jobPostsQuery.Where(j => j.Country.Id == query.Country);

        if (query.ExperienceLevels?.Count > 0)
            jobPostsQuery = jobPostsQuery.Where(
                j => j.ExperienceLevelId != null && query.ExperienceLevels.Contains(j.ExperienceLevelId.Value)
            );

        if (query.RemoteOnly is true)
            jobPostsQuery = jobPostsQuery.Where(j => j.IsRemote);

        if (query.Categories?.Count > 0)
            jobPostsQuery = jobPostsQuery.Where(j => query.Categories.Contains(j.CategoryId));

        if (query.EmploymentTypes?.Count > 0)
            jobPostsQuery = jobPostsQuery.Where(j => query.EmploymentTypes.Contains(j.EmploymentTypeId));

        if (query.PostedDate is not null)
        {
            var postedDate = DateTime.UtcNow.AddDays(-query.PostedDate.Value);
            jobPostsQuery = jobPostsQuery.Where(j => j.PublishedAt >= postedDate);
        }

        if (query.MinSalary is not null && query.MinSalary.Value > 0)
            jobPostsQuery = jobPostsQuery.Where(j =>
                j.Currency != null && j.MinSalary != null &&
                j.MinSalary.Value / j.Currency.Rate >= query.MinSalary.Value);

        return jobPostsQuery;
    }
}