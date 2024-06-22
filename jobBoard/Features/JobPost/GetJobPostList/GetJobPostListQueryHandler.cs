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
        var jobPostListQuery = appDbContext.JobPosts.AsNoTracking().Where(j => j.IsPublished && !j.IsDeleted);


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

            jobPostListQuery = jobPostListQuery.Where(predicate);
        }

        if (query.Country is not null)
            jobPostListQuery = jobPostListQuery.Where(j => query.Country == j.Country.Slug);

        if (query.Remote is not null)
            jobPostListQuery = jobPostListQuery.Where(j => j.IsRemote);

        if (query.Categories is not null && query.Categories.Count != 0)
            jobPostListQuery = jobPostListQuery.Where(j => query.Categories.Contains(j.Category.Slug));

        if (query.EmploymentTypes is not null && query.EmploymentTypes.Count != 0)
            jobPostListQuery = jobPostListQuery.Where(j => query.EmploymentTypes.Contains(j.EmploymentType.Slug));


        jobPostListQuery = jobPostListQuery.OrderByDescending(j => j.IsFeatured).ThenByDescending(j => j.PublishedAt);

        var jobPostList = await jobPostListQuery
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
                j.FeaturedStartDate.GetValueOrDefault(),
                j.FeaturedEndDate.GetValueOrDefault(),
                j.PublishedAt.GetValueOrDefault()
            ))
            .ToListAsync(cancellationToken);

        var totalJobPostCount = await jobPostListQuery.CountAsync(cancellationToken);

        return new GetJobPostListResponse(jobPostList, totalJobPostCount);
    }
}