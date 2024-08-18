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
            .Where(j => !j.IsDeleted && j.Business != null &&
                        j.Business.Members.Any(m => m.UserId == currentUserId));

        jobPostListQuery = query.Status switch
        {
            "featured" => jobPostListQuery.Where(j => j.IsPublished && j.IsFeatured),
            "published" => jobPostListQuery.Where(j => j.IsPublished && !j.IsFeatured),
            "requires_payment" => jobPostListQuery.Where(j =>
                !j.IsPublished && j.IsFeatured && j.Payments.Count != 0 &&
                j.Payments.All(p => !p.IsProcessed)),
            _ => jobPostListQuery
        };

        jobPostListQuery = jobPostListQuery.OrderByDescending(j => j.CreatedAt);

        var jobPostList = await jobPostListQuery
            .Skip((query.Page - 1) * PageSize)
            .Take(PageSize)
            .Select(j => new GetMyJobPostDto(
                j.Id,
                j.Title,
                j.Category.Label,
                j.EmploymentType.Label,
                j.Country.Label,
                j.ExperienceLevel != null ? j.ExperienceLevel.Label : null,
                j.City != null ? j.City.Label : null,
                j.IsPublished,
                j.IsFeatured,
                !j.IsPublished && j.IsFeatured && j.Payments.Count != 0 &&
                j.Payments.All(p => !p.IsProcessed),
                j.ApplyCount,
                j.FeaturedStartDate,
                j.FeaturedEndDate,
                j.PublishedAt,
                j.CreatedAt))
            .ToListAsync(cancellationToken);

        var totalJobPostCount = await jobPostListQuery.CountAsync(cancellationToken);

        return new GetMyJobPostListResponse(jobPostList, totalJobPostCount);
    }
}