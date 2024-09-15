using JobBoard.Common.Models;
using JobBoard.Domain.Common;
using JobBoard.Domain.JobPost;
using JobBoard.Features.Payment;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.PaymentService;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Slugify;

namespace JobBoard.Features.JobPost.CreateFeaturedJobPostGuest;

public class CreateFeaturedJobPostGuestCommandHandler(
    IPaymentService paymentService,
    AppDbContext dbContext,
    ILogger<CreateFeaturedJobPostGuestCommandHandler> logger
)
    : IRequestHandler<CreateFeaturedJobPostGuestCommand, Result<CreateJobPostCheckoutSessionResponse, Error>>
{
    public async Task<Result<CreateJobPostCheckoutSessionResponse, Error>> Handle(
        CreateFeaturedJobPostGuestCommand command,
        CancellationToken cancellationToken)
    {
        var jobPostId = await CreateJobPostEntity(command, cancellationToken);

        var stripeCustomerId = await paymentService.CreateStripeCustomer(command.CompanyEmail, command.CompanyName);
        var session = await paymentService.CreateCheckoutSession(stripeCustomerId, jobPostId);

        await CreateJobPostPayment(jobPostId, session.Id, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Guest company {CompanyName} created a featured job post with id: {JobPostId}",
            command.CompanyName, jobPostId);

        return new CreateJobPostCheckoutSessionResponse(session.Url);
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

    private async Task<Guid> CreateJobPostEntity(CreateFeaturedJobPostGuestCommand command,
        CancellationToken cancellationToken)
    {
        var slug = GenerateSlug(command.CompanyName, command.Title);

        var jobPost = new JobPostEntity
        {
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
            Slug = slug,
            IsFeatured = true,
            IsPublished = false,
            PublishedAt = null
        };

        await dbContext.JobPosts.AddAsync(jobPost, cancellationToken);

        await UpdateJobPostCityAsync(jobPost, command.City, cancellationToken);
        await UpdateJobPostTagsAsync(jobPost, command.Tags, cancellationToken);

        return jobPost.Id;
    }

    private static string GenerateSlug(string companyName, string title)
    {
        var slugHelper = new SlugHelper();
        var randomString = Guid.NewGuid().ToString("N")[..6];

        return slugHelper.GenerateSlug($"{randomString} {companyName} {title}");
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