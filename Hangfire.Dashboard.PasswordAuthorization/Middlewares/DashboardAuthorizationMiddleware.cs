using System.Security.Claims;
using Hangfire.Dashboard.PasswordAuthorization.EmbeddedFiles;
using Hangfire.Dashboard.PasswordAuthorization.Options;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Hangfire.Dashboard.PasswordAuthorization.Middlewares;

public class DashboardAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    private readonly PasswordAuthorizationOptions _options;


    public DashboardAuthorizationMiddleware(RequestDelegate next,
        PasswordAuthorizationOptions options)
    {
        _next = next;

        _options = options;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if ((bool)httpContext.User?.Identity?.IsAuthenticated)
        {
            httpContext.Response.StatusCode = 301;
            httpContext.Response.Redirect(DashboardAuthorizationRouteConfig.PathMatch);
            await httpContext.Response.WriteAsync("ok");
            return;
        }

        var requestUrl = httpContext.Request.Path.Value;

        if (requestUrl.Contains("css") || requestUrl.Contains("js") || requestUrl.Contains("html") ||
            requestUrl.Contains("jpg"))
        {
            await DashboardAuthorizationEmbeddedFiles.IncludeEmbeddedFile(httpContext, requestUrl);
            return;
        }


        using var scope = httpContext.RequestServices.CreateScope();
        if (httpContext.Request.Method.ToLower() == "post")
        {
            var captchaId = httpContext.Request.Form["captchaId"];
            var code = httpContext.Request.Form["code"];

            var captcha = scope.ServiceProvider.GetService<ICaptcha>();

            if (!captcha.Validate(captchaId, code))
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsJsonAsync(new { code = 401, message = "验证码错误" });
                return;
            }


            var account = httpContext.Request.Form["account"];
            var password = httpContext.Request.Form["password"];

            if (account == _options.Account && password == _options.Password)
            {
                await httpContext.SignInAsync(_options.AuthenticationScheme, new ClaimsPrincipal(
                    new ClaimsIdentity(new List<Claim>
                    {
                        new(ClaimTypes.Name, account),
                        new(ClaimTypes.Role, "job")
                    }, _options.AuthenticationScheme)));

                httpContext.Response.StatusCode = 200;
                await httpContext.Response.WriteAsJsonAsync(new { code = 200, message = "登录成功" });
                return;
            }

            httpContext.Response.StatusCode = 400;
            await httpContext.Response.WriteAsJsonAsync(new { code = 400, message = "账号或密码错误" });
        }
    }
}