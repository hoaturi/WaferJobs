using JobBoard.Common.Constants;
using JobBoard.Domain.Auth;

namespace JobBoard.Common.Interfaces;

public interface IJwtService
{
    (string accessToken, string refreshToken) GenerateTokens(ApplicationUserEntity userEntity, IList<string> roles);
    string GenerateAccessToken(ApplicationUserEntity userEntity, IList<string> roles);
    Task<bool> ValidateToken(string token, JwtTypes jwtType);
    long GetExpiration(string token);
    Guid GetUserId(string token);
}