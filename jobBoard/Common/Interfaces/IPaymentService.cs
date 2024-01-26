using Stripe.Checkout;

namespace JobBoard;

public interface IPaymentService
{
    Task<string> CreateCustomer(string email, string name);
    Task<Session> CreateFeaturedListingCheckoutSessions(string customerId, Guid jobId);
}
