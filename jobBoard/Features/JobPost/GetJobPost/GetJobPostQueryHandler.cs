using JobBoard.Common.Models;
using JobBoard.Domain.JobPost;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.GetJobPost;

public class GetJobPostQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetJobPostQuery, Result<GetJobPostResponse, Error>>
{
    public async Task<Result<GetJobPostResponse, Error>> Handle(
        GetJobPostQuery command,
        CancellationToken cancellationToken
    )
    {
        var jobPost = await dbContext
            .JobPosts.AsNoTracking()
            .Where(j => j.Slug == command.Slug && j.IsPublished)
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
                j.Slug,
                j.ExperienceLevel != null ? j.ExperienceLevel.Label : null,
                j.City != null ? j.City.Label : null,
                j.MinSalary,
                j.MaxSalary,
                j.Currency != null ? j.Currency.Code : null,
                j.Tags.Select(t => t.Label).ToList(),
                j.ApplyUrl,
                j.BusinessId,
                j.CompanyLogoUrl,
                j.CompanyWebsiteUrl,
                j.Business != null && j.Business.BusinessSize != null ? j.Business.BusinessSize.Label : null,
                j.Business != null ? j.Business.Location : null,
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