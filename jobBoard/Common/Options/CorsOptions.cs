namespace JobBoard.Common.Options;

public class CorsOptions
{
    public const string Key = "Cors";

    public required string[] AllowedOrigins { get; init; }
}