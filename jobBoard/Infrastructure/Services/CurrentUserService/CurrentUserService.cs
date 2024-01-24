using System.IdentityModel.Tokens.Jwt;

namespace JobBoard;

public class CurrentUserService(IHttpContextAccessor accessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor = accessor;

    public Guid GetUserId()
    {
        var userId =
            (
                _accessor
                    .HttpContext?.User
                    .Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)
                    ?.Value
            ) ?? throw new UserNotFoundException();

        return Guid.Parse(userId);
    }
}
