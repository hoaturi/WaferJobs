namespace JobBoard.Common.Options;

public class StripeOptions
{
    public const string Key = "Stripe";

    public required string SecretKey { get; init; }

    public required string WebhookSecret { get; init; }

    public required string FeaturedListingPriceId { get; init; }

    public required string SuccessUrl { get; init; }

    public required string CancelUrl { get; init; }
}