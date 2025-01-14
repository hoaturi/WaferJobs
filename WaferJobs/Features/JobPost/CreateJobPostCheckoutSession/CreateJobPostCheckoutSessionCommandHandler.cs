﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Models;
using WaferJobs.Domain.Business.Exceptions;
using WaferJobs.Domain.JobPost;
using WaferJobs.Domain.JobPost.Exceptions;
using WaferJobs.Features.Payment;
using WaferJobs.Infrastructure.Persistence;
using WaferJobs.Infrastructure.Services.CurrentUserService;
using WaferJobs.Infrastructure.Services.PaymentService;

namespace WaferJobs.Features.JobPost.CreateJobPostCheckoutSession;

public class CreateJobPostCheckoutSessionCommandHandler(
    AppDbContext dbContext,
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

        var jobPost = await dbContext.JobPosts
            .Include(j => j.Business)
            .Where(j => j.Id == command.Id && j.Business != null && j.Business.Memberships.Any(m => m.UserId == userId))
            .FirstOrDefaultAsync(cancellationToken);

        if (jobPost is null) throw new JobPostNotFoundException(command.Id);

        var membership = jobPost.Business!.Memberships.FirstOrDefault(m => m.UserId == userId && m.IsActive) ??
                         throw new BusinessMembershipNotFoundException(userId);

        if (membership.stripeCustomerId is null)
        {
            var stripeCustomerId =
                await paymentService.CreateStripeCustomer(membership.User.Email!, membership.FirstName);
            membership.stripeCustomerId = stripeCustomerId;
        }

        var session = await paymentService.CreateCheckoutSession(membership.stripeCustomerId, jobPost.Id);

        var payment = new JobPostPaymentEntity
            { JobPostId = jobPost.Id, CheckoutSessionId = session.Id };

        await dbContext.JobPostPayments.AddAsync(payment, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created checkout session {SessionId} for job post {JobPostId}", session.Id, jobPost.Id);

        return new CreateJobPostCheckoutSessionResponse(session.Url);
    }
}