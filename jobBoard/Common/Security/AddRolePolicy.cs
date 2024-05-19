using JobBoard.Common.Constants;
using Microsoft.AspNetCore.Authorization;

namespace JobBoard.Common.Security;

public static class RolePolicy
{
    public static void AddRolePolicy(AuthorizationOptions options, string role)
    {
        options.AddPolicy(
            role,
            policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(role, nameof(UserRoles.Admin));
            }
        );
    }
}