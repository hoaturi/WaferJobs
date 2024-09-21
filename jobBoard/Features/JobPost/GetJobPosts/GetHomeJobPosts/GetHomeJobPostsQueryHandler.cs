using JobBoard.Common.Models;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.GetJobPosts.GetHomeJobPosts;

public class GetHomeJobPostsQueryHandler(
    AppDbContext dbContext
) : IRequestHandler<GetHomeJobPostsQuery, Result<GetHomeJobPostsResponse, Error>>
{
    public async Task<Result<GetHomeJobPostsResponse, Error>> Handle(GetHomeJobPostsQuery query,
        CancellationToken cancellationToken)
    {
        var featuredJobs = await dbContext.JobPosts.AsNoTracking()
            .Where(j => j.IsFeatured && j.IsPublished)
            .OrderByDescending(j => j.PublishedAt)
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

        var latestJobs = await dbContext.JobPosts.AsNoTracking()
            .Where(j => j.IsPublished && !j.IsFeatured)
            .OrderByDescending(j => j.PublishedAt)
            .Take(20)
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

        return new GetHomeJobPostsResponse(featuredJobs, latestJobs);
    }
}