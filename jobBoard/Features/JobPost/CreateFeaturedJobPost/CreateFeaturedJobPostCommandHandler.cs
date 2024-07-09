using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
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
    ILocationService locationService,
    AppDbContext appDbContext,
    ILogger<CreateFeaturedJobPostCommandHandler> logger)
    : IRequestHandler<CreateFeaturedJobPostCommand, Result<CreateJobPostCheckoutSessionResponse, Error>>
{
    public async Task<Result<CreateJobPostCheckoutSessionResponse, Error>> Handle(
        CreateFeaturedJobPostCommand command,
        CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetUserId();
        var business = await GetBusinessByUserId(currentUserId, cancellationToken);

        var cityId = await locationService.GetOrCreateCityIdAsync(command.City, cancellationToken);
        var jobPost = MapToEntity(command, business, cityId);
        await appDbContext.JobPosts.AddAsync(jobPost, cancellationToken);

        logger.LogInformation("Creating job post: {jobPostId}", jobPost.Id);

        var stripeCustomerId = await GetOrCreateStripeCustomerId(business, command.CompanyEmail);

        var session = await paymentService.CreateCheckoutSession(stripeCustomerId, jobPost.Id);

        await CreateJobPostPayment(jobPost.Id, session.Id, cancellationToken);

        await appDbContext.SaveChangesAsync(cancellationToken);

        return new CreateJobPostCheckoutSessionResponse(session.Url);
    }

    private async Task<BusinessEntity> GetBusinessByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var business = await appDbContext.Businesses
            .FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken);

        if (business is null) throw new BusinessNotFoundForUserException(userId);

        return business;
    }


    private async Task<string> GetOrCreateStripeCustomerId(BusinessEntity business, string email)
    {
        if (business.StripeCustomerId is not null) return business.StripeCustomerId;

        var stripeCustomerId = await paymentService.CreateStripeCustomer(email, business.Name, business.Id);

        business.StripeCustomerId = stripeCustomerId;

        return stripeCustomerId;
    }

    private async Task CreateJobPostPayment(Guid jobPostId, string sessionId, CancellationToken cancellationToken)
    {
        var payment = new JobPostPaymentEntity
            { JobPostId = jobPostId, CheckoutSessionId = sessionId };

        await appDbContext.JobPostPayments.AddAsync(payment, cancellationToken);
        logger.LogInformation("Creating job post payment: {paymentId}", payment.Id);
    }

    private static JobPostEntity MapToEntity(CreateFeaturedJobPostCommand command, BusinessEntity business, int? cityId)
    {
        var jobPost = new JobPostEntity
        {
            CategoryId = command.CategoryId,
            CountryId = command.CountryId,
            CityId = cityId,
            EmploymentTypeId = command.EmploymentTypeId,
            Title = command.Title,
            Description = command.Description,
            CompanyName = command.CompanyName,
            CompanyEmail = command.CompanyEmail,
            CompanyLogoUrl = command.CompanyLogoUrl,
            CompanyWebsiteUrl = command.CompanyWebsiteUrl,
            ApplyUrl = command.ApplyUrl,
            IsRemote = command.IsRemote,
            MinSalary = command.MinSalary,
            MaxSalary = command.MaxSalary,
            Currency = command.Currency,
            BusinessId = business.Id,
            Tags = command.Tags,
            IsFeatured = true,
            IsPublished = false,
            PublishedAt = null
        };

        return jobPost;
    }
}