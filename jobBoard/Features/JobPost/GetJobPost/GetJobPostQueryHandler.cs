using JobBoard.Common.Models;
using JobBoard.Domain.JobPost;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.GetJobPost;

public class GetJobPostQueryHandler(AppDbContext appDbContext)
    : IRequestHandler<GetJobPostQuery, Result<GetJobPostResponse, Error>>
{
    public async Task<Result<GetJobPostResponse, Error>> Handle(
        GetJobPostQuery command,
        CancellationToken cancellationToken
    )
    {
        var jobPost = await appDbContext
            .JobPosts.AsNoTracking()
            .Where(j => j.Id == command.Id && j.IsPublished)
            .Include(j => j.Category)
            .Include(j => j.Country)
            .Include(j => j.EmploymentType)
            .Select(j => new GetJobPostResponse(
                j.Id,
                j.Category.Label,
                j.Country.Label,
                j.EmploymentType.Label,
                j.Title,
                j.Description,
                j.IsRemote,
                j.IsFeatured,
                j.CompanyName,
                j.City,
                j.MinSalary,
                j.MaxSalary,
                j.Currency,
                j.ApplyUrl,
                j.BusinessId,
                j.CompanyLogoUrl,
                j.Tags,
                j.PublishedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return jobPost is null
            ? JobPostErrors.JobPostNotFound(command.Id)
            : jobPost;
    }
}