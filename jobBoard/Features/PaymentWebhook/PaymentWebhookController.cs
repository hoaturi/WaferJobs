using JobBoard.Common.Extensions;
using JobBoard.Common.Options;
using JobBoard.Features.JobPost.PublishFeaturedJobPost;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace JobBoard.Features.PaymentWebhook;

[Tags("Payment Webhook")]
[ApiController]
[Route("api/payment-webhook")]
public class PaymentWebhookController(ISender sender, IOptions<StripeOptions> stripeOptions)
    : ControllerBase
{
    private readonly StripeOptions _stripeOptions = stripeOptions.Value;

    [HttpPost]
    public async Task<IActionResult> HandleWebhooks()
    {
        var eventJson = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        var stripeEvent = EventUtility.ConstructEvent(
            eventJson,
            Request.Headers["Stripe-Signature"],
            _stripeOptions.WebhookSecret
        );

        switch (stripeEvent.Type)
        {
            case Events.CheckoutSessionCompleted:
                var checkoutSession = stripeEvent.Data.Object as Session;

                var publishJobPostCommand = new PublishFeaturedJobPostCommand(
                    stripeEvent.Id, checkoutSession!.Id
                );

                var publishJobPostResult = await sender.Send(publishJobPostCommand);

                if (!publishJobPostResult.IsSuccess) return this.HandleError(publishJobPostResult.Error);

                break;
        }

        return Ok();
    }
}