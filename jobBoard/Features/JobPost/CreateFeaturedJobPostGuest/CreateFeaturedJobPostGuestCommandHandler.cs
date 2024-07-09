using System.Transactions;
using JobBoard.Common.Constants;
using JobBoard.Common.Interfaces;
using JobBoard.Common.Models;
using JobBoard.Domain.Auth;
using JobBoard.Domain.Business;
using JobBoard.Domain.JobPost;
using JobBoard.Features.Payment;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;

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

        var jobPost = CreateJobPostEntity(command, businessId, cityId);
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

    private static JobPostEntity CreateJobPostEntity(CreateFeaturedJobPostGuestCommand command, Guid? businessId,
        int? cityId)
    {
        return new JobPostEntity
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
            Tags = command.Tags,
            IsFeatured = true,
            IsPublished = false,
            PublishedAt = null
        };
    }
}