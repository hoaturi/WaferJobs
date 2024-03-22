using LinqKit;
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

        if (request.Keyword is not null)
        {
            var predicate = PredicateBuilder.New<JobPost>();

            var keywords = request.Keyword.Split(' ');

            foreach (var keyword in keywords)
            {
                predicate = predicate.Or(j => EF.Functions.ILike(j.Title, $"%{keyword}%"));
                predicate = predicate.Or(j => EF.Functions.ILike(j.Description, $"%{keyword}%"));
                predicate = predicate.Or(j => EF.Functions.ILike(j.CompanyName, $"%{keyword}%"));
                predicate = predicate.Or(
                    j => j.Tags != null && j.Tags.Any(t => EF.Functions.ILike(t, $"%{keyword}%"))
                );
            }

            query = query.Where(predicate);
        }

        if (request.Country is not null)
        {
            query = query.Where(j => request.Country == j.Country.Slug);
        }

        if (request.Remote is not null)
        {
            query = query.Where(j => j.IsRemote == true);
        }

        if (request.Categories is not null && request.Categories.Count != 0)
        {
            query = query.Where(j => request.Categories.Any(c => c == j.Category.Slug));
        }

        if (request.EmploymentTypes is not null && request.EmploymentTypes.Count != 0)
        {
            query = query.Where(j => request.EmploymentTypes.Any(c => c == j.EmploymentType.Slug));
        }

        query = query.Where(j => j.IsPublished && !j.IsDeleted);

        query = query
            .Include(j => j.Category)
            .Include(j => j.Country)
            .Include(j => j.EmploymentType);

        query = query.OrderByDescending(j => j.IsFeatured).ThenByDescending(j => j.PublishedAt);

        var jobPostList = await query
            .Skip((request.Page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => GetJobPostListQueryMapper.MapToResponse(j))
            .ToListAsync(cancellationToken);

        var totalJobPostCount = await query.CountAsync(cancellationToken);

        return new GetJobPostListResponse(jobPostList, totalJobPostCount);
    }
}
