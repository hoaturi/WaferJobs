using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Business;
using JobBoard.Domain.Common;
using JobBoard.Domain.JobPost;
using JobBoard.Features.Payment;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.LocationService;
using JobBoard.Infrastructure.Services.PaymentService;
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
        var jobPost = await MapToEntity(command, business, cityId, cancellationToken);
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

    private async Task<JobPostEntity> MapToEntity(CreateFeaturedJobPostCommand command, BusinessEntity business,
        int? cityId, CancellationToken cancellationToken)
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
            IsFeatured = true,
            IsPublished = false,
            PublishedAt = null
        };

        if (command.Tags is not null && command.Tags.Count != 0)
            jobPost.JobPostTags = await CreateOrGetExistingTags(command.Tags, cancellationToken);

        return jobPost;
    }

    private async Task<List<JobPostTagEntity>> CreateOrGetExistingTags(IEnumerable<string> tags,
        CancellationToken cancellationToken)
    {
        var jobPostTags = new List<JobPostTagEntity>();

        foreach (var tag in tags)
        {
            var normalizedTag = tag.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(normalizedTag)) continue;

            var existingTag = await appDbContext.Tags
                .FirstOrDefaultAsync(t => t.Slug == normalizedTag, cancellationToken);

            if (existingTag is null)
            {
                existingTag = new TagEntity
                {
                    Label = tag.Trim(),
                    Slug = normalizedTag
                };
                await appDbContext.Tags.AddAsync(existingTag, cancellationToken);
            }

            jobPostTags.Add(new JobPostTagEntity { Tag = existingTag });
        }

        return jobPostTags;
    }
}