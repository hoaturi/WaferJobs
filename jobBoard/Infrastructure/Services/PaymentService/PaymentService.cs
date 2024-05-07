using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace JobBoard;

public class PaymentService : IPaymentService
{
    private readonly StripeOptions _options;
    private readonly StripeClient _client;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IOptions<StripeOptions> options, ILogger<PaymentService> logger)
    {
        _options = options.Value;
        _client = new StripeClient(_options.SecretKey);
        _logger = logger;
    }

    public async Task<string> CreateCustomer(string email, string name, Guid? businessId = null)
    {
        var customerOptions = new CustomerCreateOptions { Email = email, Name = name };

        try
        {
            var customerService = new CustomerService(_client);
            var customer = await customerService.CreateAsync(customerOptions);

            if (businessId.HasValue)
            {
                _logger.LogInformation(
                    "Created Stripe customer for business: {businessId} ",
                    businessId
                );
            }
            else
            {
                _logger.LogInformation(
                    "Created Stripe customer for anonymous user: {email}",
                    email
                );
            }

            return customer.Id;
        }
        catch (StripeException)
        {
            throw new CreateCustomerFailedException();
        }
    }

    public async Task<Session> CreateFeaturedJobPostCheckoutSessions(string customerId)
    {
        var options = new SessionCreateOptions
        {
            Customer = customerId,
            LineItems = [new() { Price = _options.FeaturedListingPriceId, Quantity = 1 }],
            Mode = "payment",
            SuccessUrl = _options.SuccessUrl,
            CancelUrl = _options.CancelUrl
        };

        try
        {
            var service = new SessionService(_client);
            var session = await service.CreateAsync(options);

            _logger.LogInformation("Created Stripe checkout session: {SessionId}", session.Id);

            return session;
        }
        catch (StripeException)
        {
            throw new CreateSessionFailedException();
        }
    }
}
