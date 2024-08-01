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
                j.City != null ? j.City.Label : null,
                j.MinSalary,
                j.MaxSalary,
                j.Currency,
                j.ApplyUrl,
                j.BusinessId,
                j.CompanyLogoUrl,
                j.CompanyWebsiteUrl,
                j.Tags.Select(t => t.Label).ToList(),
                j.FeaturedStartDate.GetValueOrDefault(),
                j.FeaturedEndDate.GetValueOrDefault(),
                j.PublishedAt.GetValueOrDefault()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return jobPost is null
            ? JobPostErrors.JobPostNotFound()
            : jobPost;
    }
}