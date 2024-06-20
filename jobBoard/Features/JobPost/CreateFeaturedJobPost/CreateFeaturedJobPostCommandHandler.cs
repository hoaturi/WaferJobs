using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Domain.JobPost;
using JobBoard.Features.Payment;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.CreateFeaturedJobPost;

public class CreateFeaturedJobPostCommandHandler(
    ICurrentUserService currentUser,
    IPaymentService paymentService,
    AppDbContext appDbContext,
    ILogger<CreateFeaturedJobPostCommandHandler> logger)
    : IRequestHandler<CreateFeaturedJobPostCommand, Result<CreateJobPostCheckoutSessionResponse, Error>>
{
    public async Task<Result<CreateJobPostCheckoutSessionResponse, Error>> Handle(
        CreateFeaturedJobPostCommand command,
        CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.TryGetUserId();
        var business = await GetBusinessByUserId(currentUserId, cancellationToken);

        var jobPost = MapToEntity(command, business);
        await appDbContext.JobPosts.AddAsync(jobPost, cancellationToken);

        logger.LogInformation("Creating job post: {jobPostId}", jobPost.Id);

        var stripeCustomerId = await GetOrCreateStripeCustomerId(command, business);

        var session = await paymentService.CreateCheckoutSession(stripeCustomerId, jobPost.Id);

        await CreateJobPostPayment(jobPost.Id, session.Id, cancellationToken);

        await appDbContext.SaveChangesAsync(cancellationToken);

        return new CreateJobPostCheckoutSessionResponse(session.Url);
    }

    private async Task<BusinessEntity?> GetBusinessByUserId(Guid? userId, CancellationToken cancellationToken)
    {
        return userId is not null
            ? await appDbContext.Businesses.FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken)
            : null;
    }

    private async Task<string> GetOrCreateStripeCustomerId(CreateFeaturedJobPostCommand command,
        BusinessEntity? business)
    {
        if (business?.StripeCustomerId is not null) return business.StripeCustomerId;

        var email = business?.UserId is not null ? currentUser.GetUserEmail() : command.CompanyEmail;
        var name = business?.Name ?? command.CompanyName;
        var stripeCustomerId = await paymentService.CreateStripeCustomer(email, name, business?.Id);

        if (business is not null) business.StripeCustomerId = stripeCustomerId;

        return stripeCustomerId;
    }

    private async Task CreateJobPostPayment(Guid jobPostId, string sessionId, CancellationToken cancellationToken)
    {
        var payment = new JobPostPaymentEntity
            { JobPostId = jobPostId, CheckoutSessionId = sessionId };

        await appDbContext.JobPostPayments.AddAsync(payment, cancellationToken);
        logger.LogInformation("Creating job post payment: {paymentId}", payment.Id);
    }

    private static JobPostEntity MapToEntity(CreateFeaturedJobPostCommand command, BusinessEntity? business)
    {
        var jobPost = new JobPostEntity
        {
            CategoryId = command.CategoryId,
            CountryId = command.CountryId,
            EmploymentTypeId = command.EmploymentTypeId,
            Title = command.Title,
            Description = command.Description,
            CompanyName = command.CompanyName,
            CompanyLogoUrl = command.CompanyLogoUrl,
            CompanyWebsiteUrl = command.CompanyWebsiteUrl,
            ApplyUrl = command.ApplyUrl,
            IsRemote = command.IsRemote,
            City = command.City,
            MinSalary = command.MinSalary,
            MaxSalary = command.MaxSalary,
            Currency = command.Currency,
            BusinessId = business?.Id,
            Tags = command.Tags,
            IsFeatured = true,
            IsPublished = false
        };

        return jobPost;
    }
}