using System.Transactions;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Business;
using JobBoard.Domain.Common;
using JobBoard.Domain.JobPost;
using JobBoard.Features.Payment;
using JobBoard.Infrastructure.Persistence;
using JobBoard.Infrastructure.Services.LocationService;
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
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var businessId = await HandleSignupIfNeeded(command.SignupPayload, cancellationToken);

        var cityId = await locationService.GetOrCreateCityIdAsync(command.City, cancellationToken);

        var jobPost = await MapToEntity(command, businessId, cityId, cancellationToken);
        await appDbContext.JobPosts.AddAsync(jobPost, cancellationToken);

        logger.LogInformation("Creating guest job post with ID: {JobPostId}", jobPost.Id);

        var stripeCustomerId = await CreateStripeCustomer(command.CompanyEmail, command.CompanyName);
        var session = await paymentService.CreateCheckoutSession(stripeCustomerId, jobPost.Id);

        await CreateJobPostPayment(jobPost.Id, session.Id, cancellationToken);

        await appDbContext.SaveChangesAsync(cancellationToken);

        scope.Complete();

        return new CreateJobPostCheckoutSessionResponse(session.Url);
    }


    private async Task<Guid?> HandleSignupIfNeeded(BusinessSignupPayload? signupPayload,
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

        var businessId = await CreateBusiness(signupPayload, newUser, cancellationToken);

        logger.LogInformation("Created Business user with ID: {UserId}", newUser.Id);

        return businessId;
    }

    private async Task<Guid> CreateBusiness(BusinessSignupPayload command, ApplicationUserEntity newUserEntity,
        CancellationToken cancellationToken)
    {
        var newBusiness = new BusinessEntity
        {
            UserId = newUserEntity.Id,
            Name = command.Name!
        };

        await appDbContext.Businesses.AddAsync(newBusiness, cancellationToken);

        logger.LogInformation("Business entity created for user with email: {Email}", command.Email);

        return newBusiness.Id;
    }

    private async Task<string> CreateStripeCustomer(string email, string companyName)
    {
        return await paymentService.CreateStripeCustomer(email, companyName);
    }

    private async Task CreateJobPostPayment(Guid jobPostId, string sessionId, CancellationToken cancellationToken)
    {
        var payment = new JobPostPaymentEntity
        {
            JobPostId = jobPostId,
            CheckoutSessionId = sessionId
        };

        await appDbContext.JobPostPayments.AddAsync(payment, cancellationToken);
        logger.LogInformation("Created job post payment with ID: {PaymentId} for job post: {JobPostId}", payment.Id,
            jobPostId);
    }

    private async Task<JobPostEntity> MapToEntity(CreateFeaturedJobPostGuestCommand command, Guid? businessId,
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
            BusinessId = businessId,
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