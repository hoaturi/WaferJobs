using JobBoard.Common.Models;
using JobBoard.Domain.Common;
using JobBoard.Domain.JobPost;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CachingServices.LocationService;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.UpdateMyJobPost;

public class UpdateMyJobPostCommandHandler(
    ICurrentUserService currentUser,
    AppDbContext appDbContext,
    ILocationService locationService,
    ILogger<UpdateMyJobPostCommandHandler> logger)
    : IRequestHandler<UpdateMyJobPostCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(UpdateMyJobPostCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var jobPost = await appDbContext.JobPosts
            .Include(j => j.Business)
            .ThenInclude(b => b!.Members)
            .Include(j => j.City)
            .Include(j => j.Tags)
            .FirstOrDefaultAsync(j => j.Id == command.Id && !j.IsDeleted, cancellationToken);

        if (jobPost is null) throw new JobPostNotFoundException(command.Id);

        if (jobPost.Business!.Members.Any(m => m.UserId != userId))
            throw new UnauthorizedJobPostAccessException(command.Id, userId);

        UpdateJobPostDetailsAsync(jobPost, command.Dto);
        await UpdateJobPostTagsAsync(jobPost, command.Dto.Tags, cancellationToken);
        await UpdateJobPostCityAsync(jobPost, command.Dto.City, cancellationToken);

        logger.LogInformation("Job post with id: {Id} was updated by user with id: {UserId}", command.Id, userId);

        return Unit.Value;
    }

    private static void UpdateJobPostDetailsAsync(JobPostEntity jobPost, UpdateMyJobPostDto dto)
    {
        jobPost.CategoryId = dto.CategoryId;
        jobPost.CountryId = dto.CountryId;
        jobPost.EmploymentTypeId = dto.EmploymentTypeId;
        jobPost.Description = dto.Description;
        jobPost.Title = dto.Title;
        jobPost.CompanyName = dto.CompanyName;
        jobPost.ApplyUrl = dto.ApplyUrl;
        jobPost.IsRemote = dto.IsRemote;
        jobPost.CompanyLogoUrl = dto.CompanyLogoUrl;
        jobPost.CompanyWebsiteUrl = dto.CompanyWebsiteUrl;
        jobPost.ExperienceLevelId = dto.ExperienceLevelId;
        jobPost.MinSalary = dto.MinSalary;
        jobPost.MaxSalary = dto.MaxSalary;
        jobPost.CurrencyId = dto.CurrencyId;
    }

    private async Task UpdateJobPostCityAsync(JobPostEntity jobPost, string? cityName,
        CancellationToken cancellationToken)
    {
        if (jobPost.City?.Label != cityName && !string.IsNullOrWhiteSpace(cityName))
            jobPost.City = await locationService.GetOrCreateCityAsync(cityName, cancellationToken);
        else if (string.IsNullOrWhiteSpace(cityName)) jobPost.City = null;
    }

    private async Task UpdateJobPostTagsAsync(JobPostEntity jobPost, List<string>? tags,
        CancellationToken cancellationToken)
    {
        if (tags is null || tags.Count is 0)
        {
            jobPost.Tags.Clear();
            return;
        }

        var normalizedTags = NormalizeTags(tags);
        var existingTags = jobPost.Tags.Select(t => t.Slug).ToList();

        if (!normalizedTags.SequenceEqual(existingTags))
        {
            jobPost.Tags.Clear();

            var dbTags = await appDbContext.Tags
                .Where(t => normalizedTags.Contains(t.Slug))
                .ToListAsync(cancellationToken);

            var newTags = normalizedTags.Except(dbTags.Select(t => t.Slug))
                .Select(newTag => new TagEntity { Label = newTag, Slug = newTag }).ToList();

            dbTags.AddRange(newTags);
            jobPost.Tags = dbTags;
        }
    }

    private static List<string> NormalizeTags(List<string> tags)
    {
        return tags
            .Select(t => t.Trim().ToLowerInvariant().Replace(" ", "-"))
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .ToList();
    }
}