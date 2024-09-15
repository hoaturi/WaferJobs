using JobBoard.Common.Models;
using JobBoard.Domain.JobPost;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.DeleteMyJobPost;

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
            .ThenInclude(businessProfileEntity => businessProfileEntity!.Members)
            .FirstOrDefaultAsync(j => j.Id == command.Id, cancellationToken);

        if (jobPost is null) throw new JobPostNotFoundException(command.Id);

        if (jobPost.Business is not null && jobPost.Business.Members.Any(m => m.UserId != currentUserId))
            throw new UnauthorizedJobPostAccessException(command.Id, currentUserId);

        dbContext.Remove(jobPost);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Job post {Id} deleted by user {UserId}", command.Id, currentUserId);

        return Unit.Value;
    }
}