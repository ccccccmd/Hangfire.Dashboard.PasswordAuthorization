using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Hangfire.Dashboard.PasswordAuthorization.Authorization;

public class PasswordAuthorizationFilter : AuthorizeAttribute, IDashboardAuthorizationFilter
{
    [Authorize]
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        if (httpContext.User.Identity?.IsAuthenticated ?? false)
        {
            return true;
        }

        httpContext.Response.StatusCode = 301;
        httpContext.Response.Redirect(DashboardAuthorizationRouteConfig.LoginPath);
        httpContext.Response.WriteAsync("ok");
        return false;
    }
}