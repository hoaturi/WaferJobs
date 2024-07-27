using JobBoard.Common.Models;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.GetMyJobPostList;

public class
    GetMyJobPostListQueryHandler(
        AppDbContext appDbContext,
        ICurrentUserService currentUserService
    )
    : IRequestHandler<GetMyJobPostListQuery, Result<GetMyJobPostListResponse, Error>>
{
    private const int PageSize = 20;

    public async Task<Result<GetMyJobPostListResponse, Error>> Handle(GetMyJobPostListQuery query,
        CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetUserId();

        var jobPostListQuery = appDbContext.JobPosts.AsNoTracking()
            .Where(j => !j.IsDeleted && j.Business!.UserId == currentUserId);


        jobPostListQuery = query.Status switch
        {
            "featured" => jobPostListQuery.Where(j => j.IsPublished && j.IsFeatured),
            "published" => jobPostListQuery.Where(j => j.IsPublished && !j.IsFeatured),
            "requires_payment" => jobPostListQuery.Where(j =>
                j.Payments != null && !j.IsPublished && j.IsFeatured && j.Payments.Count != 0 &&
                j.Payments.All(p => !p.IsProcessed)),
            _ => jobPostListQuery.Where(j => j.Business!.UserId == currentUserId && j.IsDeleted == false)
        };

        jobPostListQuery = jobPostListQuery.OrderByDescending(j => j.CreatedAt);

        var jobPostList = await jobPostListQuery
            .Skip((query.Page - 1) * PageSize)
            .Take(PageSize)
            .Select(j => new GetMyJobPost(
                j.Id,
                j.Title,
                j.Category.Label,
                j.EmploymentType.Label,
                j.Country.Label,
                j.City != null ? j.City.Label : null,
                j.IsPublished,
                j.IsFeatured,
                j.Payments != null && !j.IsPublished && j.IsFeatured && j.Payments.Count != 0 &&
                j.Payments.All(p => !p.IsProcessed),
                j.FeaturedStartDate,
                j.FeaturedEndDate,
                j.PublishedAt,
                j.CreatedAt))
            .ToListAsync(cancellationToken);

        var totalJobPostCount = await jobPostListQuery.CountAsync(cancellationToken);

        return new GetMyJobPostListResponse(jobPostList, totalJobPostCount);
    }
}