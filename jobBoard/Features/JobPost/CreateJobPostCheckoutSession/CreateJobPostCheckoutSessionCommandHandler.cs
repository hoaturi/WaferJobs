using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.JobPost;
using JobBoard.Features.Payment;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.CreateJobPostCheckoutSession;

public class CreateJobPostCheckoutSessionCommandHandler(
    AppDbContext appDbContext,
    ICurrentUserService currentUserService,
    IPaymentService paymentService,
    ILogger<CreateJobPostCheckoutSessionCommandHandler> logger)
    : IRequestHandler<CreateJobPostCheckoutSessionCommand,
        Result<CreateJobPostCheckoutSessionResponse, Error>>
{
    public async Task<Result<CreateJobPostCheckoutSessionResponse, Error>> Handle(
        CreateJobPostCheckoutSessionCommand command,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetUserId();

        var jobPost = await appDbContext.JobPosts
            .AsNoTracking()
            .Include(j => j.Business)
            .Include(j => j.Payments)
            .Where(j => j.IsFeatured)
            .FirstOrDefaultAsync(j => j.Id == command.Id, cancellationToken);

        if (jobPost is null) throw new JobPostNotFoundException(command.Id);

        if (jobPost.Business is null) throw new BusinessNotFoundForUserException(userId);

        if (jobPost.Business.UserId != userId) throw new UnauthorizedJobPostAccessException(jobPost.Id, userId);

        if (jobPost.Business.StripeCustomerId is null)
            await paymentService.CreateStripeCustomer(
                jobPost.CompanyEmail,
                jobPost.Business.Name,
                jobPost.BusinessId
            );

        var session = await paymentService.CreateCheckoutSession(jobPost.Business.StripeCustomerId!, jobPost.Id);

        var payment = new JobPostPaymentEntity
            { JobPostId = jobPost.Id, CheckoutSessionId = session.Id };

        await appDbContext.JobPostPayments.AddAsync(payment, cancellationToken);
        logger.LogInformation("Creating job post payment: {paymentId}", payment.Id);

        await appDbContext.SaveChangesAsync(cancellationToken);

        return new CreateJobPostCheckoutSessionResponse(session.Url);
    }
}