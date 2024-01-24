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
            .Select(
                j =>
                    new GetJobPostResponse
                    {
                        Id = j.Id,
                        Category = j.Category,
                        Country = j.Country,
                        EmploymentType = j.EmploymentType,
                        Title = j.Title,
                        Description = j.Description,
                        IsRemote = j.IsRemote,
                        IsFeatured = j.IsFeatured,
                        BusinessId = j.BusinessId,
                        City = j.City,
                        MinSalary = j.MinSalary,
                        MaxSalary = j.MaxSalary,
                        Currency = j.Currency,
                        ApplyUrl = j.ApplyUrl,
                        CompanyName = j.CompanyName,
                        CompanyLogoUrl = j.CompanyLogoUrl,
                        PublishedAt = j.PublishedAt,
                    }
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (jobPost is null)
        {
            return JobPostErrors.JobPostNotFound(request.Id);
        }

        return jobPost;
    }
}
