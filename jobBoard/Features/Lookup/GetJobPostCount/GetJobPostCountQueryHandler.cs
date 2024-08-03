using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard.Features.Lookup.GetJobPostCount;

public class GetJobPostCountQueryHandler(
    AppDbContext dbContext,
    IDistributedCache cache
)
    : IRequestHandler<GetJobPostCountQuery, Result<GetJobPostCountResponse, Error>>
{
    public async Task<Result<GetJobPostCountResponse, Error>> Handle(GetJobPostCountQuery request,
        CancellationToken cancellationToken)
    {
        var totalCount = await cache.GetStringAsync(CacheKeys.JobCountCacheKey, cancellationToken);

        if (totalCount is not null) return new GetJobPostCountResponse(int.Parse(totalCount));

        var count = await dbContext.JobPosts.Where(
                jp => !jp.IsDeleted && jp.IsPublished
            )
            .CountAsync(cancellationToken);

        await cache.SetStringAsync(CacheKeys.JobCountCacheKey, count.ToString(), cancellationToken);

        return new GetJobPostCountResponse(count);
    }
}