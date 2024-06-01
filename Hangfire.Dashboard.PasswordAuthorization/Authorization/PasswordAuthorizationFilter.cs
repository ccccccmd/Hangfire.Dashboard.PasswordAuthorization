using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Hangfire.Dashboard.PasswordAuthorization.Authorization;

public class PasswordAuthorizationFilter : AuthorizeAttribute, IDashboardAuthorizationFilter
{
    [Authorize]
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        if ((bool)httpContext.User?.Identity?.IsAuthenticated)
        {
            return true;
        }

        httpContext.Response.StatusCode = 301;
        httpContext.Response.Redirect(DashboardAuthorizationRouteConfig.LoginPath);
        httpContext.Response.WriteAsync("ok");
        return false;
    }
}