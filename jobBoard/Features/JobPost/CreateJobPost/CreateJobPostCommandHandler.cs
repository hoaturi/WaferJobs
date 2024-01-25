using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard;

public class CreateJobPostCommandHandler(
    ICurrentUserService currentUser,
    IPaymentService paymentService,
    AppDbContext appDbContext
) : IRequestHandler<CreateJobPostCommand, Result<CreateJobPostResponse, Error>>
{
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IPaymentService _paymentService = paymentService;
    private readonly AppDbContext _appDbContext = appDbContext;

    public async Task<Result<CreateJobPostResponse, Error>> Handle(
        CreateJobPostCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = _currentUser.GetUserId();

        var business = await _appDbContext
            .Businesses.Where(b => b.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (business is not null && business.StripeCustomerId is null)
        {
            var userEmail = _currentUser.GetUserEmail();
            business.StripeCustomerId = await _paymentService.CreateCustomer(
                userEmail,
                business.Name
            );
        }

        var jobPost = new JobPost
        {
            CategoryId = request.CategoryId,
            CountryId = request.CountryId,
            EmploymentTypeId = request.EmploymentTypeId,
            Title = request.Title,
            Description = request.Description,
            CompanyName = business!.Name,
            CompanyLogoUrl = business.LogoUrl,
            ApplyUrl = request.ApplyUrl,
            IsRemote = request.IsRemote,
            IsFeatured = request.IsFeatured,
            City = request.City,
            MinSalary = request.MinSalary,
            MaxSalary = request.MaxSalary,
            Currency = request.Currency,
            BusinessId = business.Id,
            IsPublished = false
        };

        await _appDbContext.JobPosts.AddAsync(jobPost, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);

        var session = await _paymentService.CreateFeaturedListingCheckoutSessions(
            business.StripeCustomerId!,
            jobPost.Id
        );

        return new CreateJobPostResponse(session.Url);
    }
}
