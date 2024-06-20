using JobBoard.Common.Models;
using JobBoard.Domain.JobPost;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.PublishFeaturedJobPost;

public class PublishFeaturedJobPostCommandHandler(
    AppDbContext appDbContext,
    ILogger<PublishFeaturedJobPostCommandHandler> logger)
    : IRequestHandler<PublishFeaturedJobPostCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(
        PublishFeaturedJobPostCommand request,
        CancellationToken cancellationToken)
    {
        var jobPostPayment = await appDbContext.JobPostPayments
            .Include(jpp => jpp.JobPostEntity)
            .FirstOrDefaultAsync(jpp => jpp.CheckoutSessionId == request.SessionId,
                cancellationToken);

        if (jobPostPayment is null) throw new JobPostPaymentNotFoundException(request.SessionId);

        if (jobPostPayment.IsProcessed)
        {
            logger.LogWarning("Job post with id: {JobPostId} has already been processed",
                jobPostPayment.JobPostEntity!.Id);
            return Unit.Value;
        }

        jobPostPayment.JobPostEntity!.IsPublished = true;
        jobPostPayment.JobPostEntity.PublishedAt = DateTime.UtcNow;
        jobPostPayment.EventId = request.StripeEventId;
        jobPostPayment.IsProcessed = true;

        await appDbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Published job post with id: {JobPostId}", jobPostPayment.JobPostEntity!.Id);

        return Unit.Value;
    }
}