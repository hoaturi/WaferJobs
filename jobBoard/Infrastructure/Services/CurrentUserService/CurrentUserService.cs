using System.IdentityModel.Tokens.Jwt;
using JobBoard.Common.Exceptions;
using JobBoard.Common.Interfaces;

namespace JobBoard.Infrastructure.Services.CurrentUserService;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid GetUserId()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId)) throw new InvalidJwtException();
        return userId;
    }

    public Guid? TryGetUserId()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId)) return userId;
        return null;
    }

    public string GetUserEmail()
    {
        var emailClaim = httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Email);
        if (emailClaim == null) throw new InvalidJwtException();
        return emailClaim.Value;
    }
}