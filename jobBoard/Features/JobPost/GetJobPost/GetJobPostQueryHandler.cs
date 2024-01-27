using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard;

public class GetJobPostQueryHandler(AppDbContext appDbContext)
    : IRequestHandler<GetJobPostQuery, Result<GetJobPostResponse, Error>>
{
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<Result<GetJobPostResponse, Error>> Handle(
        GetJobPostQuery request,
        CancellationToken cancellationToken
    )
    {
        var jobPost = await _appDbContext
            .JobPosts.AsNoTracking()
            .Where(j => j.Id == request.Id && j.IsPublished)
            .Include(j => j.Category)
            .Include(j => j.Country)
            .Include(j => j.EmploymentType)
            .Select(j => GetJobPostQueryMapper.MapToResponse(j))
            .FirstOrDefaultAsync(cancellationToken);

        if (jobPost is null)
        {
            return JobPostErrors.JobPostNotFound(request.Id);
        }

        return jobPost;
    }
}
