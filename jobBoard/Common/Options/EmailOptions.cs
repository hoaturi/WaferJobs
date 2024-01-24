using System.ComponentModel.DataAnnotations;

namespace JobBoard;

public record class EmailOptions
{
    public const string key = "Email";

    [Required]
    public required string FromAddress { get; init; }

    [Required]
    public required string BaseUrl { get; init; }

    [Required]
    public required string ResetPasswordPath { get; init; }

    public string ResetPasswordUrl => $"https://{BaseUrl}{ResetPasswordPath}";
}
