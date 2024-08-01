using System.Transactions;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Common;
using JobBoard.Domain.JobPost;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.LocationService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.UpdateMyJobPost;

public class UpdateMyJobPostCommandHandler(
    ICurrentUserService currentUser,
    ILocationService locationService,
    AppDbContext appDbContext,
    ILogger<UpdateMyJobPostCommandHandler> logger)
    : IRequestHandler<UpdateMyJobPostCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(UpdateMyJobPostCommand command, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var jobPost = await appDbContext.JobPosts
            .Include(j => j.Business)
            .Where(j => j.Id == command.Id && !j.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (jobPost == null) throw new JobPostNotFoundException(command.Id);

        if (jobPost.Business is null) throw new BusinessNotFoundForUserException(userId);

        if (jobPost.Business.UserId != userId) throw new UnauthorizedJobPostAccessException(command.Id, userId);

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            await UpdateJobPost(command, jobPost, cancellationToken);
            scope.Complete();
        }

        logger.LogInformation("Job post with id: {Id} was updated by user with id: {UserId}", command.Id, userId);

        return Unit.Value;
    }

    private async Task UpdateJobPost(UpdateMyJobPostCommand command, JobPostEntity jobPost,
        CancellationToken cancellationToken)
    {
        jobPost.CategoryId = command.Dto.CategoryId;
        jobPost.CountryId = command.Dto.CountryId;
        jobPost.EmploymentTypeId = command.Dto.EmploymentTypeId;
        jobPost.Description = command.Dto.Description;
        jobPost.Title = command.Dto.Title;
        jobPost.CompanyName = command.Dto.CompanyName;
        jobPost.ApplyUrl = command.Dto.ApplyUrl;
        jobPost.IsRemote = command.Dto.IsRemote;
        jobPost.CompanyLogoUrl = command.Dto.CompanyLogoUrl;
        jobPost.CompanyWebsiteUrl = command.Dto.CompanyWebsiteUrl;
        jobPost.MinSalary = command.Dto.MinSalary;
        jobPost.MaxSalary = command.Dto.MaxSalary;
        jobPost.Currency = command.Dto.Currency;

        if (!string.IsNullOrWhiteSpace(command.Dto.City))
        {
            var cityId = await locationService.GetOrCreateCityIdAsync(command.Dto.City, cancellationToken);
            jobPost.CityId = cityId;
        }
        else
        {
            jobPost.CityId = null;
        }

        if (command.Dto.Tags is not null && command.Dto.Tags.Count > 0)
        {
            var loweredTags = command.Dto.Tags.Select(t => t.ToLower()).ToList();

            var tags = await appDbContext.Tags
                .Where(t => loweredTags.Contains(t.Slug.ToLower()))
                .ToListAsync(cancellationToken);

            jobPost.Tags = tags;
        }
        else
        {
            jobPost.Tags = new List<TagEntity>();
        }


        await appDbContext.SaveChangesAsync(cancellationToken);
    }
}