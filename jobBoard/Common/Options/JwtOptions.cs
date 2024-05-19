namespace JobBoard.Common.Options;

public class JwtOptions
{
    public const string Key = "Jwt";

    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    public required string AccessKey { get; init; }

    public required string RefreshKey { get; init; }

    public required string AccessExpires { get; init; }

    public required string RefreshExpires { get; init; }
}