using JobBoard.Common.Models;
using JobBoard.Domain.Business;
using JobBoard.Domain.Common;
using JobBoard.Domain.JobPost;
using JobBoard.Features.Payment;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.LookupServices.LocationService;
using JobBoard.Infrastructure.Services.PaymentService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.CreateFeaturedJobPostGuest;

public class CreateFeaturedJobPostGuestCommandHandler(
    IPaymentService paymentService,
    ILocationService locationService,
    AppDbContext dbContext,
    ILogger<CreateFeaturedJobPostGuestCommandHandler> logger)
    : IRequestHandler<CreateFeaturedJobPostGuestCommand, Result<CreateJobPostCheckoutSessionResponse, Error>>
{
    public async Task<Result<CreateJobPostCheckoutSessionResponse, Error>> Handle(
        CreateFeaturedJobPostGuestCommand command,
        CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var business = await GetExistingBusiness(command, cancellationToken);

            var jobPostId = await CreateJobPostEntity(command, business?.Id, cancellationToken);

            var stripeCustomerId =
                await GetOrCreateStripeCustomerId(command.CompanyEmail, command.CompanyName, business);
            var session = await paymentService.CreateCheckoutSession(stripeCustomerId, jobPostId);

            await CreateJobPostPayment(jobPostId, session.Id, cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation("Company {CompanyName} created a featured job post with id: {JobPostId}",
                command.CompanyName, jobPostId);

            return new CreateJobPostCheckoutSessionResponse(session.Url);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create a featured job post for company {CompanyName}", command.CompanyName);
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task<BusinessEntity?> GetExistingBusiness(CreateFeaturedJobPostGuestCommand command,
        CancellationToken cancellationToken)
    {
        if (command.BusinessId is null) return null;

        var business = await dbContext.Businesses
            .FirstOrDefaultAsync(b => b.Id == command.BusinessId, cancellationToken);

        if (business is null) throw new BusinessNotFoundException(command.BusinessId.Value);

        return business;
    }

    private async Task<string> GetOrCreateStripeCustomerId(string email, string companyName, BusinessEntity? business)
    {
        if (business is null) return await paymentService.CreateStripeCustomer(email, companyName);

        return business.StripeCustomerId ??
               (business.StripeCustomerId = await paymentService.CreateStripeCustomer(email, companyName));
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

    private async Task<Guid> CreateJobPostEntity(CreateFeaturedJobPostGuestCommand command, Guid? businessId,
        CancellationToken cancellationToken)
    {
        var jobPost = new JobPostEntity
        {
            BusinessId = businessId,
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

        return jobPost.Id;
    }

    private async Task UpdateJobPostCityAsync(JobPostEntity jobPost, string? cityName,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(cityName))
            jobPost.City = await locationService.GetOrCreateCityAsync(cityName, cancellationToken);
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