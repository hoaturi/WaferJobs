using JobBoard.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace JobBoard.Infrastructure.Services.PaymentService;

public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly StripeClient _stripeClient;
    private readonly StripeOptions _stripeOptions;

    public PaymentService(IOptions<StripeOptions> options, ILogger<PaymentService> logger)
    {
        _stripeOptions = options.Value;
        _stripeClient = new StripeClient(_stripeOptions.SecretKey);
        _logger = logger;
    }

    public async Task<string> CreateStripeCustomer(string email, string name, Guid? businessId = null)
    {
        var customerOptions = new CustomerCreateOptions
        {
            Email = email,
            Name = name
        };

        try
        {
            var customerService = new CustomerService(_stripeClient);
            var customer = await customerService.CreateAsync(customerOptions);

            if (businessId.HasValue)
                _logger.LogDebug("Created Stripe customer for business: {BusinessId}", businessId);
            else
                _logger.LogDebug("Created Stripe customer for anonymous user: {Email}", email);

            return customer.Id;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to create Stripe customer");
            throw new CreateStripeCustomerFailedException();
        }
    }


    public async Task<Session> CreateCheckoutSession(string customerId, Guid jobPostId)
    {
        var sessionOptions = new SessionCreateOptions
        {
            Customer = customerId,
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Price = _stripeOptions.FeaturedListingPriceId,
                    Quantity = 1
                }
            ],
            Mode = "payment",
            SuccessUrl = _stripeOptions.SuccessUrl + "/" + jobPostId,
            CancelUrl = _stripeOptions.CancelUrl
        };

        try
        {
            var sessionService = new SessionService(_stripeClient);
            var session = await sessionService.CreateAsync(sessionOptions);

            _logger.LogDebug("Created Stripe checkout session: {SessionId}", session.Id);

            return session;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to create Stripe checkout session");
            throw new CreateSessionFailedException();
        }
    }
}