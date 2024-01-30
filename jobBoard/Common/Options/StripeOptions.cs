using System.ComponentModel.DataAnnotations;

namespace JobBoard;

public class StripeOptions
{
    public const string Key = "Stripe";

    [Required]
    public required string SecretKey { get; init; }

    [Required]
    public required string WebhookSecret { get; init; }

    [Required]
    public required string FeaturedListingPriceId { get; init; }

    [Required]
    public required string SuccessUrl { get; init; }

    [Required]
    public required string CancelUrl { get; init; }
}
