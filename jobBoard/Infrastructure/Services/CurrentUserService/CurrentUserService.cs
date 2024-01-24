using System.IdentityModel.Tokens.Jwt;

namespace JobBoard;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor = accessor;

    public Guid GetUserId()
    {
        var userId = (
            _accessor
                .HttpContext?.User
                .Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)
                ?.Value ?? throw new InvalidJwtException()
        );

        return Guid.Parse(userId);
    }

    public Guid? TryGetUserId()
    {
        var userId = (
            _accessor
                .HttpContext?.User
                .Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)
                ?.Value
        );

        if (userId is null)
        {
            return null;
        }

        return Guid.Parse(userId);
    }

    public string GetUserEmail()
    {
        var email = (
            _accessor
                .HttpContext?.User
                .Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)
                ?.Value ?? throw new InvalidJwtException()
        );

        return email;
    }
}
