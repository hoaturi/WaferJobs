using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Domain.Business.Exceptions;
using JobBoard.Domain.Common;
using JobBoard.Domain.JobPost;
using JobBoard.Features.Payment;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.CurrentUserService;
using JobBoard.Infrastructure.Services.PaymentService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.CreateFeaturedJobPost;

public class CreateFeaturedJobPostCommandHandler(
    AppDbContext dbContext,
    ICurrentUserService currentUser,
    ILogger<CreateFeaturedJobPostCommandHandler> logger,
    IPaymentService paymentService)
    : IRequestHandler<CreateFeaturedJobPostCommand,
        Result<CreateJobPostCheckoutSessionResponse, Error>>
{
    public async Task<Result<CreateJobPostCheckoutSessionResponse, Error>> Handle(
        CreateFeaturedJobPostCommand command,
        CancellationToken cancellationToken)
    {
        var currentUserId = currentUser.GetUserId();
        var business = await GetBusinessByUserId(currentUserId, cancellationToken);

        var jobPost = await CreateJobPostEntity(command, business, cancellationToken);

        var stripeCustomerId = await GetOrCreateStripeCustomerId(business, command.CompanyEmail);
        var session = await paymentService.CreateCheckoutSession(stripeCustomerId, jobPost.Id);

        await CreateJobPostPayment(jobPost.Id, session.Id, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("{business} created a featured job post with id: {jobPostId}",
            business.Name, jobPost.Id);

        return new CreateJobPostCheckoutSessionResponse(session.Url);
    }

    private async Task<BusinessEntity> GetBusinessByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var business = await dbContext.Businesses
            .FirstOrDefaultAsync(b => b.Members.Any(m => m.UserId == userId), cancellationToken);

        if (business is null) throw new BusinessNotFoundForUserException(userId);

        return business;
    }

    private async Task<string> GetOrCreateStripeCustomerId(BusinessEntity business, string email)
    {
        if (business.StripeCustomerId is not null) return business.StripeCustomerId;

        var stripeCustomerId =
            await paymentService.CreateStripeCustomer(email, business.Name, business.Id);
        business.StripeCustomerId = stripeCustomerId;

        return stripeCustomerId;
    }

    private async Task CreateJobPostPayment(Guid jobPostId, string sessionId, CancellationToken cancellationToken)
    {
        var payment = new JobPostPaymentEntity
        {
            JobPostId = jobPostId,
            CheckoutSessionId = sessionId
        };

        await dbContext.JobPostPayments.AddAsync(payment, cancellationToken);
    }

    private async Task<JobPostEntity> CreateJobPostEntity(CreateFeaturedJobPostCommand command,
        BusinessEntity business,
        CancellationToken cancellationToken)
    {
        var jobPost = new JobPostEntity
        {
            BusinessId = business.Id,
            CategoryId = command.CategoryId,
            CountryId = command.CountryId,
            EmploymentTypeId = command.EmploymentTypeId,
            ExperienceLevelId = command.ExperienceLevelId,
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
            CurrencyId = command.CurrencyId,
            IsFeatured = true,
            IsPublished = false,
            PublishedAt = null
        };

        await dbContext.JobPosts.AddAsync(jobPost, cancellationToken);

        await UpdateJobPostCityAsync(jobPost, command.City, cancellationToken);
        await UpdateJobPostTagsAsync(jobPost, command.Tags, cancellationToken);

        return jobPost;
    }

    private async Task UpdateJobPostCityAsync(JobPostEntity jobPost, string? cityName,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(cityName))
        {
            var normalizedCity = cityName.Trim().ToLowerInvariant().Replace(" ", "-");

            var city = await dbContext.Cities
                           .FirstOrDefaultAsync(c => c.Slug == normalizedCity, cancellationToken) ??
                       new CityEntity { Label = cityName, Slug = normalizedCity };

            jobPost.City = city;
        }
    }

    private async Task UpdateJobPostTagsAsync(JobPostEntity jobPost, List<string>? tags,
        CancellationToken cancellationToken)
    {
        if (tags is null || tags.Count is 0) return;

        var normalizedTags = NormalizeTags(tags);

        var dbTags = await dbContext.Tags
            .Where(t => normalizedTags.Select(nt => nt.Slug).Contains(t.Slug))
            .ToListAsync(cancellationToken);

        var newTags = normalizedTags
            .Where(nt => dbTags.All(t => t.Slug != nt.Slug))
            .Select(nt => new TagEntity { Label = nt.Label, Slug = nt.Slug })
            .ToList();

        await dbContext.Tags.AddRangeAsync(newTags, cancellationToken);
        jobPost.Tags = dbTags.Concat(newTags).ToList();
    }

    private static List<(string Label, string Slug)> NormalizeTags(List<string> tags)
    {
        return tags
            .Select(t => (
                Label: t.Trim(),
                Slug: t.Trim().ToLowerInvariant().Replace(" ", "-")
            ))
            .Where(t => !string.IsNullOrWhiteSpace(t.Slug))
            .DistinctBy(t => t.Slug)
            .ToList();
    }
}