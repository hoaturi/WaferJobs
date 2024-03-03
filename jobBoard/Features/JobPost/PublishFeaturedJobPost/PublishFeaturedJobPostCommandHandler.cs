using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard;

public class PublishFeaturedJobPostCommandHandler(
    AppDbContext appDbContext,
    ILogger<PublishFeaturedJobPostCommandHandler> logger
) : IRequestHandler<PublishFeaturedJobPostCommand, Result<Unit, Error>>
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ILogger<PublishFeaturedJobPostCommandHandler> _logger = logger;

    public async Task<Result<Unit, Error>> Handle(
        PublishFeaturedJobPostCommand request,
        CancellationToken cancellationToken
    )
    {
        var jobPostPayment =
            await _appDbContext
                .JobPostPayments.Where(jpp => jpp.CheckoutSessionId == request.SessionId)
                .Include(jpp => jpp.JobPost)
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new JobPostPaymentNotFoundException();

        if (jobPostPayment.IsProcessed)
        {
            _logger.LogInformation(
                "Job post with id {JobPostId} has already been processed",
                jobPostPayment.JobPost.Id
            );
            return Unit.Value;
        }

        PublishJobPost(jobPostPayment, request.StripeEventId);

        await _appDbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Published job post with id {JobPostId}", jobPostPayment.JobPost.Id);

        return Unit.Value;
    }

    private static JobPostPayment PublishJobPost(
        JobPostPayment jobPostPayment,
        string stripeEventId
    )
    {
        jobPostPayment.JobPost.IsPublished = true;
        jobPostPayment.JobPost.PublishedAt = DateTime.UtcNow;
        jobPostPayment.EventId = stripeEventId;
        jobPostPayment.IsProcessed = true;

        return jobPostPayment;
    }
}
