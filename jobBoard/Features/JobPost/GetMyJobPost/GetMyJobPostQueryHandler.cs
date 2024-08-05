using JobBoard.Common.Models;
using JobBoard.Domain.JobPost;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.GetMyJobPost;

public class GetMyJobPostQueryHandler(
    ICurrentUserService currentUserService,
    AppDbContext appDbContext)
    : IRequestHandler<GetMyJobPostQuery, Result<GetMyJobPostResponse, Error>>
{
    public async Task<Result<GetMyJobPostResponse, Error>> Handle(GetMyJobPostQuery query,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var jobPost = await appDbContext.JobPosts
            .AsNoTracking()
            .Where(j => j.Id == query.Id && !j.IsDeleted)
            .Select(j => new GetMyJobPostDto(
                j.Id,
                j.Category.Label,
                j.Country.Label,
                j.EmploymentType.Label,
                j.Title,
                j.Description,
                j.IsRemote,
                j.IsFeatured,
                j.CompanyName,
                j.CompanyEmail,
                j.ExperienceLevel != null ? j.ExperienceLevel.Label : null,
                j.City != null ? j.City.Label : null,
                j.MinSalary,
                j.MaxSalary,
                j.Currency != null ? j.Currency.Label : null,
                j.ApplyUrl,
                j.CompanyLogoUrl,
                j.CompanyWebsiteUrl,
                j.Tags.Select(t => t.Label).ToList(),
                j.Business != null ? j.Business.UserId : null
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (jobPost is null) throw new JobPostNotFoundException(query.Id);

        if (jobPost.UserId != userId) throw new UnauthorizedJobPostAccessException(query.Id, userId);

        return MapToResponse(jobPost);
    }

    private static GetMyJobPostResponse MapToResponse(GetMyJobPostDto dto)
    {
        return new GetMyJobPostResponse(
            dto.Id,
            dto.Category,
            dto.Country,
            dto.EmploymentType,
            dto.Title,
            dto.Description,
            dto.IsRemote,
            dto.IsFeatured,
            dto.CompanyName,
            dto.CompanyEmail,
            dto.ExperienceLevel,
            dto.City,
            dto.MinSalary,
            dto.MaxSalary,
            dto.Currency,
            dto.ApplyUrl,
            dto.CompanyLogoUrl,
            dto.CompanyWebsiteUrl,
            dto.Tags
        );
    }
}