using Stripe.Checkout;

namespace JobBoard;

public interface IPaymentService
{
    Task<string> CreateCustomer(string email, string name, Guid? businessId = null);
    Task<Session> CreateFeaturedJobPostCheckoutSessions(string customerId);
}
