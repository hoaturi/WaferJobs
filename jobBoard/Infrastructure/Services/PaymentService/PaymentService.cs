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

    public async Task<string> CreateStripeCustomer(string email, string name, Guid? businessId)
    {
        var customerOptions = new CustomerCreateOptions
        {
            Email = email,
            Name = name
        };

        var customerService = new CustomerService(_stripeClient);
        var customer = await customerService.CreateAsync(customerOptions);

        _logger.LogInformation("Stripe customer created. CustomerId: {CustomerId}, Email: {Email}, BusinessId: {BusinessId}", 
            customer.Id, email, businessId);

        return customer.Id;
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

        var sessionService = new SessionService(_stripeClient);
        var session = await sessionService.CreateAsync(sessionOptions);

        _logger.LogInformation("Stripe checkout session created. SessionId: {SessionId}, CustomerId: {CustomerId}, JobPostId: {JobPostId}", 
            session.Id, customerId, jobPostId);

        return session;
    }
}