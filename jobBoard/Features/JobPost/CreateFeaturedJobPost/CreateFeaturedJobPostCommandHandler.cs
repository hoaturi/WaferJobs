using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobBoard;

public class CreateFeaturedJobPostCommandHandler(
    ICurrentUserService currentUser,
    IPaymentService paymentService,
    AppDbContext appDbContext,
    ILogger<CreateFeaturedJobPostCommandHandler> logger
) : IRequestHandler<CreateFeaturedJobPostCommand, Result<CreateFeaturedJobPostResponse, Error>>
{
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IPaymentService _paymentService = paymentService;
    private readonly AppDbContext _appDbContext = appDbContext;
    private readonly ILogger<CreateFeaturedJobPostCommandHandler> _logger = logger;

    public async Task<Result<CreateFeaturedJobPostResponse, Error>> Handle(
        CreateFeaturedJobPostCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = _currentUser.TryGetUserId();

        Business? business = null;
        if (userId is not null)
        {
            business = await _appDbContext
                .Businesses.Where(b => b.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        var jobPost = CreateFeaturedJobPostCommandMapper.MapToEntity(request, business);
        await _appDbContext.JobPosts.AddAsync(jobPost, cancellationToken);

        _logger.LogInformation("Creating job post: {jobPostId}", jobPost.Id);

        string? stripeCustomerId = null;
        if (business != null)
        {
            await CreateStripeCustomerIfNotExists(business);
            stripeCustomerId = business.StripeCustomerId;
        }
        else
        {
            stripeCustomerId = await _paymentService.CreateCustomer(
                request.CompanyEmail,
                request.CompanyName
            );
        }

        var session = await _paymentService.CreateFeaturedJobPostCheckoutSessions(
            stripeCustomerId!
        );

        await CreateJobPostPayment(jobPost.Id, session.Id, cancellationToken);

        await _appDbContext.SaveChangesAsync(cancellationToken);

        return new CreateFeaturedJobPostResponse(session.Url);
    }

    public async Task CreateStripeCustomerIfNotExists(Business business)
    {
        if (business.StripeCustomerId is not null)
        {
            return;
        }

        var userEmail = _currentUser.GetUserEmail();
        business.StripeCustomerId = await _paymentService.CreateCustomer(
            userEmail,
            business.Name,
            business.Id
        );
    }

    private async Task<JobPostPayment> CreateJobPostPayment(
        Guid jobPostId,
        string sessionId,
        CancellationToken cancellationToken
    )
    {
        var payment = new JobPostPayment { JobPostId = jobPostId, CheckoutSessionId = sessionId, };

        await _appDbContext.JobPostPayments.AddAsync(payment, cancellationToken);
        _logger.LogInformation("Creating job post payment: {paymentId}", payment.Id);

        return payment;
    }
}
