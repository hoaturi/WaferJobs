﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Models;
using WaferJobs.Domain.JobPost.Exceptions;
using WaferJobs.Infrastructure.Persistence;
using WaferJobs.Infrastructure.Services.CurrentUserService;

namespace WaferJobs.Features.JobPost.DeleteMyJobPost;

public class DeleteMyJobPostCommandHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUserService,
    ILogger<DeleteMyJobPostCommand> logger) : IRequestHandler<DeleteMyJobPostCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(DeleteMyJobPostCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetUserId();

        var jobPost = await dbContext.JobPosts
            .Include(j => j.Business)
            .ThenInclude(businessProfileEntity => businessProfileEntity!.Memberships)
            .FirstOrDefaultAsync(j => j.Id == command.Id, cancellationToken);

        if (jobPost is null) throw new JobPostNotFoundException(command.Id);

        if (jobPost.Business is not null && jobPost.Business.Memberships.Any(m => m.UserId != currentUserId))
            throw new UnauthorizedJobPostAccessException(currentUserId, command.Id);

        dbContext.Remove(jobPost);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Deleted job post {JobPostId} for company {CompanyId} by user {UserId}", command.Id,
            jobPost.Business?.Id, currentUserId);
        return Unit.Value;
    }
}