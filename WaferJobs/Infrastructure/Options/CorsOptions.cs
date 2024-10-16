using System.ComponentModel.DataAnnotations;

namespace WaferJobs.Infrastructure.Options;

public class CorsOptions
{
    public const string Key = "Cors";
    public const string PolicyName = "CorsPolicy";

    [Required] public required string[] AllowedOrigins { get; init; }
}