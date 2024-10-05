using System.ComponentModel.DataAnnotations;

namespace JobBoard.Infrastructure.Options;

public class CorsOptions
{
    public const string Key = "Cors";
    public const string PolicyName = "CorsPolicy";

    [Required] public required string[] AllowedOrigins { get; init; }
}