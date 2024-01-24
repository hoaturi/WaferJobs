using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JobBoard;

public class JwtService(IOptions<JwtOptions> options) : IJwtService
{
    private readonly JwtOptions _options = options.Value;

    public string GenerateAccessToken(ApplicationUser user, List<string> roles)
    {
        var claims = GetClaims(user);

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var signingCredentials = GetSigningCredentials(_options.AccessKey);
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims, JwtTypes.AccessToken);
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    public string GenerateRefreshToken(ApplicationUser user)
    {
        var claims = GetClaims(user);
        var signingCredentials = GetSigningCredentials(_options.RefreshKey);
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims, JwtTypes.RefreshToken);
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    public async Task<bool> ValidateRefreshToken(string Token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_options.RefreshKey);

        var validationResult = await tokenHandler.ValidateTokenAsync(
            Token,
            new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidAudience = _options.Audience,
                ValidIssuer = _options.Issuer,
            }
        );

        return validationResult.IsValid;
    }

    public long GetExpiration(string Token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var parsedToken = tokenHandler.ReadJwtToken(Token);
        var expClaim = parsedToken
            .Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!
            .Value;

        var exp = long.Parse(expClaim);

        return exp;
    }

    public string GetUserId(string Token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var parsedToken = tokenHandler.ReadJwtToken(Token);
        var userId = parsedToken
            .Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)!
            .Value;

        return userId;
    }

    private static List<Claim> GetClaims(ApplicationUser user)
    {
        return
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
        ];
    }

    private static SigningCredentials GetSigningCredentials(string secretKey)
    {
        var key = Encoding.UTF8.GetBytes(secretKey);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private JwtSecurityToken GenerateTokenOptions(
        SigningCredentials signingCredentials,
        List<Claim> claims,
        JwtTypes jwtType = JwtTypes.AccessToken
    )
    {
        var expires = jwtType switch
        {
            JwtTypes.AccessToken => DateTime.Now.AddMinutes(double.Parse(_options.AccessExpires)),
            JwtTypes.RefreshToken => DateTime.Now.AddDays(double.Parse(_options.RefreshExpires)),
            _ => throw new ArgumentException("Invalid JWT type", nameof(jwtType))
        };

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            expires: expires,
            signingCredentials: signingCredentials
        );

        return token;
    }
}
