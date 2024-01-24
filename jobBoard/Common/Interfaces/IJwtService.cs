using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace JobBoard;

public interface IJwtService
{
    public string GenerateAccessToken(ApplicationUser user, List<string> roles);

    public string GenerateRefreshToken(ApplicationUser user);

    public Task<bool> ValidateRefreshToken(string Token);

    public long GetExpiration(string Token);

    public string GetUserId(string Token);
}
