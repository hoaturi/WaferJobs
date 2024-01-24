using Microsoft.AspNetCore.Authorization;

namespace JobBoard;

public static class RolePolicy
{
    public const string Admin = "Admin";
    public const string Business = "Business";
    public const string JobSeeker = "JobSeeker";

    public static void AddRolePolicy(AuthorizationOptions options, string role)
    {
        var requiredRoles =
            role == RoleTypes.Admin.ToString()
                ? new[] { RoleTypes.Admin.ToString() }
                : new[] { role, RoleTypes.Admin.ToString() };

        options.AddPolicy(
            role,
            policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(requiredRoles);
            }
        );
    }
}
