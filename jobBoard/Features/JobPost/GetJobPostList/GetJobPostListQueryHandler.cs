using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard;

public class GetJobPostListQueryHandler(AppDbContext appDbContext)
    : IRequestHandler<GetJobPostListQuery, Result<GetJobPostListResponse, Error>>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<Result<GetJobPostListResponse, Error>> Handle(
        GetJobPostListQuery request,
        CancellationToken cancellationToken
    )
    {
        var pageSize = 20;

        var query = _appDbContext.JobPosts.AsNoTracking();

        if (request.CategoryId is not 0)
        {
            query = query.Where(j => j.CategoryId == request.CategoryId);
        }
        if (request.CountryId is not 0)
        {
            query = query.Where(j => j.CountryId == request.CountryId);
        }
        if (request.EmploymentTypeId is not 0)
        {
            query = query.Where(j => j.EmploymentTypeId == request.EmploymentTypeId);
        }

        if (request.Keyword is not null)
        {
            var keyword = request.Keyword.ToLower();

            query = query.Where(
                j =>
                    EF.Functions.ILike(j.Title, $"%{keyword}%")
                    || EF.Functions.ILike(j.Description, $"%{keyword}%")
                    || EF.Functions.ILike(j.CompanyName, $"%{keyword}%")
                    || j.Tags != null && j.Tags.Any(t => EF.Functions.ILike(t, $"%{keyword}%"))
            );
        }

        query = query.Where(j => j.IsPublished);

        query = query
            .Include(j => j.Category)
            .Include(j => j.Country)
            .Include(j => j.EmploymentType);

        query = query.OrderByDescending(j => j.IsFeatured).ThenByDescending(j => j.PublishedAt);

        // Pagination
        var jobPostList = await query
            .Skip((request.Page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => GetJobPostListQueryMapper.MapToResponse(j))
            .ToListAsync(cancellationToken);

        var totalJobPostCount = await query.CountAsync(cancellationToken);

        return new GetJobPostListResponse(jobPostList, totalJobPostCount);
    }
}
