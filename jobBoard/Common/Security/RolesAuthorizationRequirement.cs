using Microsoft.AspNetCore.Authorization;

namespace JobBoard;

public class RolesAuthorizationRequirement(string[] allowedRoles) : IAuthorizationRequirement
{
    public string[] AllowedRoles { get; } = allowedRoles;
}
