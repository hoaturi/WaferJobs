using WaferJobs.Common.Constants;
using WaferJobs.Domain.Auth;

namespace WaferJobs.Infrastructure.Services.JwtService;

public interface IJwtService
{
    (string accessToken, string refreshToken) GenerateTokens(ApplicationUserEntity userEntity, IList<string> roles);
    string GenerateAccessToken(ApplicationUserEntity userEntity, IList<string> roles);
    Task<bool> ValidateToken(string token, JwtTypes jwtType);
    Task RevokeRefreshToken(string refreshToken, CancellationToken cancellationToken);
    Task<bool> IsRefreshTokenRevoked(string refreshToken, CancellationToken cancellationToken);
    Guid GetUserId(string token);
}