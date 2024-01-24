using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard;

public record GetJobPostQuery(Guid Id) : IRequest<Result<GetJobPostResponse, Error>>;

public record GetJobPostResponse(
    Guid Id,
    string TItle,
    string Description,
    bool IsRemote,
    bool IsFeatured,
    Guid? BusinessId,
    string? City,
    int? MinSalary,
    int? MaxSalary,
    string? Currency,
    string? ApplyUrl,
    string CompanyName,
    string? CompanyLogoUrl
);

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
            .JobPosts.Where(j => j.Id == request.Id)
            .Include(j => j.Business)
            .Include(j => j.Category)
            .Include(j => j.Country)
            .Include(j => j.EmploymentType)
            .Select(
                j =>
                    new GetJobPostResponse(
                        j.Id,
                        j.Title,
                        j.Description,
                        j.IsRemote,
                        j.IsFeatured,
                        j.BusinessId,
                        j.City,
                        j.MinSalary,
                        j.MaxSalary,
                        j.Currency,
                        j.ApplyUrl,
                        j.CompanyName,
                        j.CompanyLogoUrl
                    )
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (jobPost is null)
        {
            return JobPostErrors.JobPostNotFound;
        }

        return jobPost;
    }
}
