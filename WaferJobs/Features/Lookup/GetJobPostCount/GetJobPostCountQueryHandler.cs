using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Models;
using WaferJobs.Infrastructure.Persistence;

namespace WaferJobs.Features.Lookup.GetJobPostCount;

public class GetJobPostCountQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetJobPostCountQuery, Result<GetJobPostCountResponse, Error>>
{
    public async Task<Result<GetJobPostCountResponse, Error>> Handle(GetJobPostCountQuery request,
        CancellationToken cancellationToken)
    {
        var count = await dbContext.JobPosts.Where(
                jp => jp.IsPublished
            )
            .CountAsync(cancellationToken);

        return new GetJobPostCountResponse(count);
    }
}