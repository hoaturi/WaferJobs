using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Domain.JobPost;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.GetMyJobPost;

public class
    GetMyJobPostQueryHandler(
        ICurrentUserService currentUserService,
        AppDbContext appDbContext)
    : IRequestHandler<GetMyJobPostQuery,
        Result<GetMyJobPostResponse, Error>>
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
                j.City != null ? j.City.Label : null,
                j.MinSalary,
                j.MaxSalary,
                j.Currency,
                j.ApplyUrl,
                j.BusinessId,
                j.CompanyLogoUrl,
                j.CompanyWebsiteUrl,
                j.Tags,
                j.Business != null ? j.Business.UserId : null
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (jobPost == null) throw new JobPostNotFoundException(query.Id);

        if (jobPost.UserId != userId) throw new UnauthorizedJobPostAccessException(query.Id, userId);

        return MapToResponse(jobPost);
    }

    private static GetMyJobPostResponse MapToResponse(GetMyJobPostDto getMyJobPost)
    {
        return new GetMyJobPostResponse(
            getMyJobPost.Id,
            getMyJobPost.Category,
            getMyJobPost.Country,
            getMyJobPost.EmploymentType,
            getMyJobPost.Title,
            getMyJobPost.Description,
            getMyJobPost.IsRemote,
            getMyJobPost.IsFeatured,
            getMyJobPost.CompanyName,
            getMyJobPost.City,
            getMyJobPost.MinSalary,
            getMyJobPost.MaxSalary,
            getMyJobPost.Currency,
            getMyJobPost.ApplyUrl,
            getMyJobPost.BusinessId,
            getMyJobPost.CompanyLogoUrl,
            getMyJobPost.CompanyWebsiteUrl,
            getMyJobPost.Tags
        );
    }
}