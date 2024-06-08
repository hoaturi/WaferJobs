using JobBoard.Common.Models;
using JobBoard.Domain.JobPost;
using JobBoard.Features.JobPost.GetJobPost;
using JobBoard.Infrastructure.Persistence;
using LinqKit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.GetJobPostList;

public class GetJobPostListQueryHandler(AppDbContext appDbContext)
    : IRequestHandler<GetJobPostListQuery, Result<GetJobPostListResponse, Error>>
{
    private const int PageSize = 20;

    public async Task<Result<GetJobPostListResponse, Error>> Handle(GetJobPostListQuery query,
        CancellationToken cancellationToken)
    {
        var jobPostsQuery = appDbContext.JobPosts.AsNoTracking();

        if (query.Keyword is not null)
        {
            var predicate = PredicateBuilder.New<JobPostEntity>();
            var keywords = query.Keyword.Split(' ');

            //  ILIKE is Postgresql specific !! 
            predicate = keywords.Aggregate(predicate, (current, keyword) => current
                .Or(j => EF.Functions.ILike(j.Title, $"%{keyword}%"))
                .Or(j => EF.Functions.ILike(j.Description, $"%{keyword}%"))
                .Or(j => EF.Functions.ILike(j.CompanyName, $"%{keyword}%"))
                .Or(j => j.Tags != null && j.Tags.Any(t => EF.Functions.ILike(t, $"%{keyword}%"))));

            jobPostsQuery = jobPostsQuery.Where(predicate);
        }

        if (query.Country is not null)
            jobPostsQuery = jobPostsQuery.Where(j => query.Country == j.Country.Slug);

        if (query.Remote is not null)
            jobPostsQuery = jobPostsQuery.Where(j => j.IsRemote);

        if (query.Categories is not null && query.Categories.Count != 0)
            jobPostsQuery = jobPostsQuery.Where(j => query.Categories.Contains(j.Category.Slug));

        if (query.EmploymentTypes is not null && query.EmploymentTypes.Count != 0)
            jobPostsQuery = jobPostsQuery.Where(j => query.EmploymentTypes.Contains(j.EmploymentType.Slug));

        jobPostsQuery = jobPostsQuery.Where(j => j.IsPublished && !j.IsDeleted)
            .Include(j => j.Category)
            .Include(j => j.Country)
            .Include(j => j.EmploymentType)
            .OrderByDescending(j => j.IsFeatured)
            .ThenByDescending(j => j.PublishedAt);

        var jobPostList = await jobPostsQuery
            .Skip((query.Page - 1) * PageSize)
            .Take(PageSize)
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
                j.City,
                j.MinSalary,
                j.MaxSalary,
                j.Currency,
                null,
                j.BusinessId,
                j.CompanyLogoUrl,
                j.CompanyWebsiteUrl,
                j.Tags,
                j.PublishedAt
            ))
            .ToListAsync(cancellationToken);

        var totalJobPostCount = await jobPostsQuery.CountAsync(cancellationToken);

        return new GetJobPostListResponse(jobPostList, totalJobPostCount);
    }
}