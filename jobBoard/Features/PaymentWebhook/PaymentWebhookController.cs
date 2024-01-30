using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace JobBoard;

[Tags("Payment Webhook")]
[ApiController]
[Route("api/payment-webhook")]
public class PaymentWebhookController(ISender sender, IOptions<StripeOptions> stripeOptions)
    : BaseController
{
    private readonly ISender _sender = sender;
    private readonly StripeOptions _stripeOptions = stripeOptions.Value;

    [HttpPost]
    public async Task<IActionResult> HandleWebhooks()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        var stripeEvent = EventUtility.ConstructEvent(
            json,
            Request.Headers["Stripe-Signature"],
            _stripeOptions.WebhookSecret
        );

        switch (stripeEvent.Type)
        {
            case Events.CheckoutSessionCompleted:

                var session = stripeEvent.Data.Object as Session;

                var command = new PublishFeaturedJobPostCommand(stripeEvent.Id, session!.Id);

                var result = await _sender.Send(command);

                if (!result.IsSuccess)
                {
                    return HandleFailure(result.Error);
                }

                break;
        }

        return Ok();
    }
}
