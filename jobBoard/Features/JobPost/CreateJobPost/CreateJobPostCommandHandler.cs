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

        if (business is not null && business.StripeCustomerId is null)
        {
            var userEmail = _currentUser.GetUserEmail();
            business.StripeCustomerId = await _paymentService.CreateCustomer(
                userEmail,
                business.Name
            );

            _logger.LogInformation(
                "Created Stripe customer for business: {businessId} ",
                business.Id
            );
        }

        var jobPost = CreateJobPostCommandMapper.MapToJobPost(request, business);

        await _appDbContext.JobPosts.AddAsync(jobPost, cancellationToken);
        await _appDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("created job post: {jobPostId}", jobPost.Id);

        var session = await _paymentService.CreateFeaturedListingCheckoutSessions(
            business!.StripeCustomerId!,
            jobPost.Id
        );

        _logger.LogInformation("Created Stripe checkout session: {SessionId}", session.Id);

        return new CreateJobPostResponse(session.Url);
    }
}
