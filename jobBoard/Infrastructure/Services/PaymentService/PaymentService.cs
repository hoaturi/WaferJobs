using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace JobBoard;

public class PaymentService : IPaymentService
{
    private readonly StripeOptions _options;
    private readonly StripeClient _client;

    public PaymentService(IOptions<StripeOptions> options)
    {
        _options = options.Value;
        _client = new StripeClient(_options.SecretKey);
    }

    public async Task<string> CreateCustomer(string email, string name)
    {
        var customerOptions = new CustomerCreateOptions { Email = email, Name = name };

        try
        {
            var customerService = new CustomerService(_client);
            var customer = await customerService.CreateAsync(customerOptions);

            return customer.Id;
        }
        catch (StripeException)
        {
            throw new CreateCustomerFailedException();
        }
    }

    public async Task<Session> CreateFeaturedListingCheckoutSessions(string customerId, Guid jobId)
    {
        var options = new SessionCreateOptions
        {
            Customer = customerId,
            LineItems = [new() { Price = _options.FeaturedListingPriceId, Quantity = 1 }],
            Mode = "payment",
            Metadata = new Dictionary<string, string> { { "JobId", jobId.ToString() } },
            SuccessUrl = _options.SuccessUrl,
            CancelUrl = _options.CancelUrl
        };

        try
        {
            var service = new SessionService(_client);
            var session = await service.CreateAsync(options);
            return session;
        }
        catch (StripeException)
        {
            throw new CreateSessionFailedException();
        }
    }
}
