﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Models;
using WaferJobs.Domain.JobPost;
using WaferJobs.Infrastructure.Persistence;
using WaferJobs.Infrastructure.Services.CurrentUserService;

namespace WaferJobs.Features.JobPost.GetMyJobPost;

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
            .Where(j => j.Slug == query.Slug && j.Business != null &&
                        j.Business.Memberships.Any(m => m.UserId == userId))
            .Select(j => new GetMyJobPostResponse(
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
                j.Tags.Select(t => t.Label).ToList()
            ))
            .FirstOrDefaultAsync(cancellationToken);

        return jobPost is null
            ? JobPostErrors.JobPostNotFound()
            : jobPost;
    }
}