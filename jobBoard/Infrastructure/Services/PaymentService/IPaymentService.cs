using Stripe.Checkout;

namespace JobBoard.Infrastructure.Services.PaymentService;

public interface IPaymentService
{
    Task<string> CreateStripeCustomer(string email, string name, Guid? businessId = null);
    Task<Session> CreateCheckoutSession(string customerId, Guid jobPostId);
}