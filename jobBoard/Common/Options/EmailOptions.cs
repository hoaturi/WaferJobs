using System.ComponentModel.DataAnnotations;

namespace JobBoard;

public record class EmailOptions
{
    public const string key = "Email";

    [Required]
    public required string FromAddress { get; init; }

    [Required]
    public required string BaseUrl { get; init; }

    public string GetPasswordResetLink(string token, string userId)
    {
        return $"http://{BaseUrl}/auth/reset-password?token={token}&userId={userId}";
    }
}
