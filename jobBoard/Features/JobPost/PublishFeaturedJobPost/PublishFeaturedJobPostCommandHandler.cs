using JobBoard.Common.Models;
using JobBoard.Domain.JobPost;
using JobBoard.Domain.JobPost.Exceptions;
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
            .Include(jpp => jpp.JobPost)
            .FirstOrDefaultAsync(jpp => jpp.CheckoutSessionId == request.SessionId,
                cancellationToken);

        if (jobPostPayment is null) throw new JobPostPaymentNotFoundException(request.SessionId);

        if (jobPostPayment.JobPost is null) throw new JobPostNotFoundException(jobPostPayment.JobPostId);

        if (jobPostPayment.IsProcessed)
        {
            logger.LogWarning("Job post {JobPostId} has already been processed", jobPostPayment.JobPost.Id);
            return Unit.Value;
        }

        jobPostPayment.JobPost.IsPublished = true;
        jobPostPayment.JobPost.PublishedAt = DateTime.UtcNow;
        jobPostPayment.JobPost.FeaturedStartDate = DateTime.UtcNow;
        jobPostPayment.JobPost.FeaturedEndDate = DateTime.UtcNow.AddDays(35);

        jobPostPayment.EventId = request.StripeEventId;
        jobPostPayment.IsProcessed = true;

        await appDbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Published featured job post {JobPostId}", jobPostPayment.JobPost.Id);

        return Unit.Value;
    }
}