using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobBoard.Common.Constants;
using JobBoard.Domain.Auth;
using JobBoard.Infrastructure.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JobBoard.Infrastructure.Services.JwtService;

public class JwtService(IOptions<JwtOptions> options, IDistributedCache cache) : IJwtService
{
    private readonly JwtOptions _jwtOptions = options.Value;

    public string GenerateAccessToken(ApplicationUserEntity userEntity, IList<string> roles)
    {
        return GenerateToken(userEntity, roles, JwtTypes.AccessToken);
    }

    public async Task<bool> ValidateToken(string token, JwtTypes jwtType)
    {
        var key = jwtType == JwtTypes.AccessToken ? _jwtOptions.AccessKey : _jwtOptions.RefreshKey;
        return await ValidateToken(token, key);
    }

    public Guid GetUserId(string token)
    {
        var userId = ReadToken(token).Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
        return Guid.Parse(userId);
    }

    public async Task RevokeRefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.RevokedToken + refreshToken;
        var cacheExpirationInSeconds = GetExpiration(refreshToken) - DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        await cache.SetStringAsync(
            cacheKey,
            RefreshTokenStatus.Revoked.ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheExpirationInSeconds)
            },
            cancellationToken
        );
    }

    public async Task<bool> IsRefreshTokenRevoked(string refreshToken, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeys.RevokedToken + refreshToken;
        return await cache.GetStringAsync(cacheKey, cancellationToken) is not null;
    }

    public (string accessToken, string refreshToken) GenerateTokens(ApplicationUserEntity userEntity,
        IList<string> roles)
    {
        var accessToken = GenerateToken(userEntity, roles, JwtTypes.AccessToken);
        var refreshToken = GenerateToken(userEntity, roles, JwtTypes.RefreshToken);
        return (accessToken, refreshToken);
    }

    private static long GetExpiration(string token)
    {
        var expClaim = ReadToken(token).Claims.First(x => x.Type == JwtRegisteredClaimNames.Exp).Value;
        return long.Parse(expClaim);
    }

    private string GenerateToken(ApplicationUserEntity userEntity, IList<string> roles, JwtTypes jwtType)
    {
        var claims = CreateClaims(userEntity);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var signingCredentials =
            GetSigningCredentials(jwtType == JwtTypes.AccessToken ? _jwtOptions.AccessKey : _jwtOptions.RefreshKey);
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims, jwtType);
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims,
        JwtTypes jwtType)
    {
        var expires = jwtType == JwtTypes.AccessToken
            ? DateTime.Now.AddMinutes(double.Parse(_jwtOptions.AccessExpireMinutes))
            : DateTime.Now.AddDays(double.Parse(_jwtOptions.RefreshExpireDays));

        return new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: expires,
            signingCredentials: signingCredentials
        );
    }

    private static List<Claim> CreateClaims(ApplicationUserEntity userEntity)
    {
        return
        [
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, userEntity.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, userEntity.Email!)
        ];
    }

    private static SigningCredentials GetSigningCredentials(string secretKey)
    {
        var key = Encoding.UTF8.GetBytes(secretKey);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<bool> ValidateToken(string token, string key)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = Encoding.UTF8.GetBytes(key);

        var validationResult = await tokenHandler.ValidateTokenAsync(
            token,
            new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidAudience = _jwtOptions.Audience,
                ValidIssuer = _jwtOptions.Issuer
            }
        );

        return validationResult.IsValid;
    }

    private static JwtSecurityToken ReadToken(string token)
    {
        return new JwtSecurityTokenHandler().ReadJwtToken(token);
    }
}