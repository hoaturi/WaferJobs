namespace JobBoard;

public interface IJwtService
{
    public string GenerateAccessToken(ApplicationUser user, List<string> roles);

    public string GenerateRefreshToken(ApplicationUser user);

    public (string accessToken, string refreshToken) GenerateTokens(
        ApplicationUser user,
        IList<string> roles
    );

    public Task<bool> ValidateRefreshToken(string Token);

    public long GetExpiration(string Token);

    public string GetUserId(string Token);
}
