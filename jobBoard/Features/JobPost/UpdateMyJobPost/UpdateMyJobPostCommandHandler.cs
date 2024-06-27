using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.JobPost;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.UpdateMyJobPost;

public class UpdateMyJobPostCommandHandler(
    ICurrentUserService currentUser,
    AppDbContext appDbContext,
    ILogger<UpdateMyJobPostCommandHandler> logger
) : IRequestHandler<UpdateMyJobPostCommand, Result<Unit, Error>>
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

        await UpdateJobPost(command, jobPost, cancellationToken);
        logger.LogInformation("Job post with id: {Id} was updated by user with id: {UserId}", command.Id, userId);

        return Unit.Value;
    }

    private async Task UpdateJobPost(UpdateMyJobPostCommand command, JobPostEntity jobPost,
        CancellationToken cancellationToken)
    {
        jobPost.CategoryId = command.CategoryId;
        jobPost.CountryId = command.CountryId;
        jobPost.EmploymentTypeId = command.EmploymentTypeId;
        jobPost.Description = command.Description;
        jobPost.Title = command.Title;
        jobPost.CompanyName = command.CompanyName;
        jobPost.ApplyUrl = command.ApplyUrl;
        jobPost.IsRemote = command.IsRemote;
        jobPost.CompanyLogoUrl = command.CompanyLogoUrl;
        jobPost.CompanyWebsiteUrl = command.CompanyWebsiteUrl;
        jobPost.City = command.City;
        jobPost.MinSalary = command.MinSalary;
        jobPost.MaxSalary = command.MaxSalary;
        jobPost.Currency = command.Currency;
        jobPost.Tags = command.Tags;

        await appDbContext.SaveChangesAsync(cancellationToken);
    }
}