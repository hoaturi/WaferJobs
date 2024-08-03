using System.ComponentModel.DataAnnotations;

namespace JobBoard.Infrastructure.Options;

public class JwtOptions
{
    public const string Key = "Jwt";

    [Required] public required string Issuer { get; init; }
    [Required] public required string Audience { get; init; }
    [Required] public required string AccessKey { get; init; }
    [Required] public required string RefreshKey { get; init; }
    [Required] public required string AccessExpires { get; init; }
    [Required] public required string RefreshExpires { get; init; }
}