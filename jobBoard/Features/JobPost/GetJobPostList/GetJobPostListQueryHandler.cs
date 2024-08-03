using JobBoard.Common.Models;
using JobBoard.Features.JobPost.GetJobPost;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.GetJobPostList;

public class GetJobPostListQueryHandler(AppDbContext appDbContext)
    : IRequestHandler<GetJobPostListQuery, Result<GetJobPostListResponse, Error>>
{
    public async Task<Result<GetJobPostListResponse, Error>> Handle(GetJobPostListQuery query,
        CancellationToken cancellationToken)
    {
        var jobPostListQuery = appDbContext.JobPosts.AsNoTracking().Where(j => j.IsPublished && !j.IsDeleted);


        if (query.Keyword is not null)
        {
            var keyword = query.Keyword.ToLower();

            jobPostListQuery = jobPostListQuery.Where(
                jp => jp.SearchVector.Matches(keyword));
        }


        if (query.City is not null)
            jobPostListQuery = jobPostListQuery.Where(j => j.City != null && j.City.Slug == query.City);

        if (query.Country is not null)
            jobPostListQuery = jobPostListQuery.Where(j => j.Country.Slug == query.Country);

        if (query.RemoteOnly == "true")
            jobPostListQuery = jobPostListQuery.Where(j => j.IsRemote);

        if (query.Categories is not null && query.Categories.Count != 0)
            jobPostListQuery = jobPostListQuery.Where(j => query.Categories.Contains(j.Category.Slug));

        if (query.EmploymentTypes is not null && query.EmploymentTypes.Count != 0)
            jobPostListQuery = jobPostListQuery.Where(j => query.EmploymentTypes.Contains(j.EmploymentType.Slug));

        if (query.PostedDate is not null)
        {
            var postedDate = DateTime.UtcNow.AddDays(-query.PostedDate.Value);
            jobPostListQuery = jobPostListQuery.Where(j => j.PublishedAt >= postedDate);
        }

        jobPostListQuery = query.FeaturedOnly switch
        {
            "true" => jobPostListQuery.Where(j => j.IsFeatured),
            "false" => jobPostListQuery.Where(j => !j.IsFeatured),
            _ => jobPostListQuery
        };

        jobPostListQuery = jobPostListQuery.OrderByDescending(j => j.IsFeatured).ThenByDescending(j => j.PublishedAt);

        var jobPostList = await jobPostListQuery
            .Skip((query.Page - 1) * query.Take)
            .Take(query.Take)
            .Select(j => new GetJobPostResponse(
                j.Id,
                j.Category.Label,
                j.Country.Label,
                j.EmploymentType.Label,
                j.Title,
                j.Description,
                j.IsRemote,
                j.IsFeatured,
                j.CompanyName,
                j.City != null ? j.City.Label : null,
                j.MinSalary,
                j.MaxSalary,
                j.Currency,
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

        var totalJobPostCount = await jobPostListQuery.CountAsync(cancellationToken);

        return new GetJobPostListResponse(jobPostList, totalJobPostCount);
    }
}