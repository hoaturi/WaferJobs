namespace JobBoard.Common.Options;

public class EmailOptions
{
    public const string Key = "Email";
    public required string FromAddress { get; init; }

    public required string BaseUrl { get; init; }

    public string GetPasswordResetLink(string token, string userId)
    {
        return $"http://{BaseUrl}/auth/reset-password?token={token}&userId={userId}";
    }
}