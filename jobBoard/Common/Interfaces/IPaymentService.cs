using Stripe.Checkout;

namespace JobBoard.Common.Interfaces;

public interface IPaymentService
{
    Task<string> CreateStripeCustomer(string email, string name, Guid? businessId = null);
    Task<Session> CreateCheckoutSession(string customerId);
}