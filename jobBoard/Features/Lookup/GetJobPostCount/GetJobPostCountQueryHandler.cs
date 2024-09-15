using JobBoard.Common.Models;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.Lookup.GetJobPostCount;

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