using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JobBoard.Common.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class ConditionalAuthorizationAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authHeader = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader is null || !authHeader.StartsWith("Bearer ")) return;
        var authResult = context
            .HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme)
            .Result;
        if (!authResult.Succeeded) context.Result = new UnauthorizedResult();
    }
}