using Hangfire.Annotations;
using Hangfire.Dashboard.PasswordAuthorization.Authorization;
using Hangfire.Dashboard.PasswordAuthorization.Middlewares;
using Hangfire.Dashboard.PasswordAuthorization.Options;
using Lazy.Captcha.Core;
using Lazy.Captcha.Core.Generator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Hangfire.Dashboard.PasswordAuthorization;

public static class DashboardAuthorizationExtension
{
    public static IServiceCollection AddLogDashboard(this IServiceCollection services,
        Action<PasswordAuthorizationOptions> passwordAuthorizationOptionFunc,
        Action<DashboardOptions> dashboardOptionFunc = null)
    {
        var dashboardOptions = new DashboardOptions
        {
            Authorization = new[]
            {
                new PasswordAuthorizationFilter()
            }
        };
        if (dashboardOptionFunc != null)
            dashboardOptionFunc(dashboardOptions);

        services.AddSingleton(dashboardOptions);
        var options = new PasswordAuthorizationOptions();
        if (passwordAuthorizationOptionFunc != null)
            passwordAuthorizationOptionFunc(options);


        services.AddSingleton(options);

        services.AddMemoryCache();
        services.AddCaptcha(option =>
        {
            option.CaptchaType = CaptchaType.WORD_NUMBER_UPPER;
            option.ImageOption.BubbleCount = 2;
            option.ImageOption.BubbleMinRadius = 2;
            option.ImageOption.BubbleMaxRadius = 6;
            option.ImageOption.InterferenceLineCount = 2;
            option.ExpirySeconds = 300;
            option.IgnoreCase = true;
            option.CodeLength = 4;
        });

        return services;
    }


    public static IApplicationBuilder UseHangfireDashboardWithLogin(
        [NotNull] this IApplicationBuilder app,
        [NotNull] string pathMatch = "/hangfire",
        [CanBeNull] JobStorage storage = null)
    {
        if (app == null) throw new ArgumentNullException(nameof(app));
        if (pathMatch == null) throw new ArgumentNullException(nameof(pathMatch));

        HangfireServiceCollectionExtensions.ThrowIfNotConfigured(app.ApplicationServices);


        DashboardAuthorizationRouteConfig.PathMatch = pathMatch;

        var services = app.ApplicationServices;

        var dashboardOptions = services.GetService<DashboardOptions>();

        app.Map(new PathString(DashboardAuthorizationRouteConfig.CaptchaPath),
            builder =>
            {
                builder.Run(async context =>
                {
                    var captcha = context.RequestServices.GetRequiredService<ICaptcha>();

                    var captchaId = context.Request.Query["captchaId"];

                    var info = captcha.Generate(captchaId);
                    var stream = new MemoryStream(info.Bytes);
                    context.Response.ContentType = "image/gif";

                    await stream.CopyToAsync(context.Response.Body).ConfigureAwait(false);
                });
            });

        app.Map(new PathString(DashboardAuthorizationRouteConfig.AuthorizationPath),
            builder => { builder.UseMiddleware<DashboardAuthorizationMiddleware>(); });

        app.UseHangfireDashboard(options: dashboardOptions, pathMatch: pathMatch, storage: storage);
        return app;
    }
}