using Stripe.Checkout;

namespace JobBoard;

public interface IPaymentService
{
    Task<string> CreateCustomer(Guid businessId, string email, string name);
    Task<Session> CreateFeaturedListingCheckoutSessions(string customerId);
}
