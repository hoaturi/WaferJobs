using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard;

public class CreateJobPostCommandHandler(
    ICurrentUserService currentUser,
    IPaymentService paymentService,
    AppDbContext appDbContext,
    ILogger<CreateJobPostCommandHandler> logger
) : IRequestHandler<CreateJobPostCommand, Result<CreateJobPostResponse, Error>>
{
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IPaymentService _paymentService = paymentService;
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ILogger<CreateJobPostCommandHandler> _logger = logger;

    public async Task<Result<CreateJobPostResponse, Error>> Handle(
        CreateJobPostCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = _currentUser.GetUserId();

        var business = await _appDbContext
            .Businesses.Where(b => b.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (business is null)
        {
            BusinessErrors.AssociatedBusinessNotFound(userId);
            _logger.LogError("Business not found for user: {userId}", userId);
        }

        await CreateStripeCustomerIfNotExists(business!);

        var jobPost = CreateJobPostCommandMapper.MapToEntity(request, business!);

        await _appDbContext.JobPosts.AddAsync(jobPost, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created job post: {jobPostId}", jobPost.Id);

        var session = await _paymentService.CreateFeaturedListingCheckoutSessions(
            business!.StripeCustomerId!,
            jobPost.Id
        );

        return new CreateJobPostResponse(session.Url);
    }

    public async Task CreateStripeCustomerIfNotExists(Business business)
    {
        if (business.StripeCustomerId is not null)
        {
            return;
        }

        var userEmail = _currentUser.GetUserEmail();
        business.StripeCustomerId = await _paymentService.CreateCustomer(
            business.Id,
            userEmail,
            business.Name
        );
    }
}
