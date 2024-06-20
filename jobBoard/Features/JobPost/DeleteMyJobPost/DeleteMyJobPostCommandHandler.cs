using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Domain.JobPost;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.DeleteMyJobPost;

public class DeleteMyJobPostCommandHandler(
    ICurrentUserService currentUserService,
    AppDbContext appDbContext,
    ILogger<DeleteMyJobPostCommand> logger) : IRequestHandler<DeleteMyJobPostCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(DeleteMyJobPostCommand command, CancellationToken cancellationToken)
    {
        var currentUserId = currentUserService.GetUserId();

        var jobPost =
            await appDbContext.JobPosts.SingleAsync(j =>
                j.Id == command.Id && j.Business!.UserId == currentUserId, cancellationToken);

        if (jobPost.IsDeleted)
            throw new JobPostAlreadyDeletedException(command.Id);

        jobPost.IsDeleted = true;

        await appDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Job post {Id} deleted by user {UserId}", command.Id, currentUserId);

        return Unit.Value;
    }
}