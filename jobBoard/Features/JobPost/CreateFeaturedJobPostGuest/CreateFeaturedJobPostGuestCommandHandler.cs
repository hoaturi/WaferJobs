using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Business;
using JobBoard.Domain.Common;
using JobBoard.Domain.JobPost;
using JobBoard.Features.Payment;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.LookupServices.LocationService;
using JobBoard.Infrastructure.Services.PaymentService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JobBoard.Features.JobPost.CreateFeaturedJobPostGuest;

public class CreateFeaturedJobPostGuestCommandHandler(
    UserManager<ApplicationUserEntity> userManager,
    IPaymentService paymentService,
    ILocationService locationService,
    AppDbContext appDbContext,
    ILogger<CreateFeaturedJobPostGuestCommandHandler> logger)
    : IRequestHandler<CreateFeaturedJobPostGuestCommand, Result<CreateJobPostCheckoutSessionResponse, Error>>
{
    public async Task<Result<CreateJobPostCheckoutSessionResponse, Error>> Handle(
        CreateFeaturedJobPostGuestCommand command,
        CancellationToken cancellationToken)
    {
        await using var transaction = await appDbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var business = await CreateNewUserIfNeeded(command.SignupPayload, cancellationToken);

            var jobPostId = await CreateJobPostEntity(command, business?.Id, cancellationToken);

            var stripeCustomerId =
                await GetOrCreateStripeCustomerId(command.CompanyEmail, command.CompanyName, business);
            var session = await paymentService.CreateCheckoutSession(stripeCustomerId, jobPostId);

            await CreateJobPostPayment(jobPostId, session.Id, cancellationToken);

            await appDbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            if (business is not null)
                logger.LogInformation(
                    "New business {BusinessName} signed up and created a featured job post with id: {JobPostId}",
                    business.Name, jobPostId);
            else
                logger.LogInformation("Guest company {CompanyName} created a featured job post with id: {JobPostId}",
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

    private async Task<BusinessEntity?> CreateNewUserIfNeeded(BusinessSignupPayload? signupPayload,
        CancellationToken cancellationToken)
    {
        if (signupPayload is null) return null;

        var existingUser = await userManager.FindByEmailAsync(signupPayload.Email);
        if (existingUser != null) throw new UserAlreadyExistsException(signupPayload.Email);

        var newUser = new ApplicationUserEntity
        {
            UserName = signupPayload.Email,
            Email = signupPayload.Email
        };

        await userManager.CreateAsync(newUser, signupPayload.Password);
        await userManager.AddToRoleAsync(newUser, nameof(UserRoles.Business));

        var business = await CreateBusinessAsync(signupPayload, newUser, cancellationToken);

        logger.LogInformation("Created Business user with Id: {UserId}", newUser.Id);

        return business;
    }

    private async Task<BusinessEntity> CreateBusinessAsync(BusinessSignupPayload payload,
        ApplicationUserEntity userEntity,
        CancellationToken cancellationToken)
    {
        var newBusiness = new BusinessEntity
        {
            UserId = userEntity.Id,
            Name = payload.Name!
        };

        await appDbContext.Businesses.AddAsync(newBusiness, cancellationToken);
        return newBusiness;
    }

    private async Task<string> GetOrCreateStripeCustomerId(string email, string companyName, BusinessEntity? business)
    {
        var stripeCustomerId = await paymentService.CreateStripeCustomer(email, companyName, business?.Id);

        if (business is not null)
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

        await appDbContext.JobPostPayments.AddAsync(payment, cancellationToken);
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

        await appDbContext.JobPosts.AddAsync(jobPost, cancellationToken);

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

        var dbTags = await appDbContext.Tags
            .Where(t => normalizedTags.Contains(t.Slug))
            .ToListAsync(cancellationToken);

        var newTags = normalizedTags.Except(dbTags.Select(t => t.Slug))
            .Select(newTag => new TagEntity { Label = newTag, Slug = newTag }).ToList();

        await appDbContext.Tags.AddRangeAsync(newTags, cancellationToken);
        jobPost.Tags = dbTags.Concat(newTags).ToList();
    }

    private static List<string> NormalizeTags(List<string> tags)
    {
        return tags
            .Select(t => t.Trim().ToLowerInvariant().Replace(" ", "-"))
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .ToList();
    }
}