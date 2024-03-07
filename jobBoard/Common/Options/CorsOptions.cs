using System.ComponentModel.DataAnnotations;

namespace JobBoard;

public record CorsOptions
{
    public const string Key = "Cors";

    [Required]
    public required string[] AllowedOrigins { get; init; }
}
