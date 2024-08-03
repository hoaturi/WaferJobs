using System.ComponentModel.DataAnnotations;

namespace JobBoard.Infrastructure.Options;

public class CorsOptions
{
    public const string Key = "Cors";

    [Required] public required string[] AllowedOrigins { get; init; }
}