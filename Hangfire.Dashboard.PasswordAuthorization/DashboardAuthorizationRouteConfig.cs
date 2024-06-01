namespace Hangfire.Dashboard.PasswordAuthorization;

internal static class DashboardAuthorizationRouteConfig
{
    private const string AuthorizationPrefix = "/authorization";

    internal static string PathMatch { get; set; } = "/hangfire";

    internal static string LoginPath => $"{PathMatch}{AuthorizationPrefix}/login.html";
    internal static string AuthorizationPath => $"{PathMatch}{AuthorizationPrefix}";
    internal static string CaptchaPath => $"{PathMatch}{AuthorizationPrefix}/captcha";
}