using Hangfire.Annotations;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Hangfire.Dashboard.PasswordAuthorization.Options;

public class PasswordAuthorizationOptions
{
    public string Account { get; set; }
    public string Password { get; set; }

    [NotNull] public string SecurityKey { get; set; }

    public TimeSpan Expire { get; set; } = TimeSpan.FromDays(1);

    public string AuthenticationScheme { get; set; } = CookieAuthenticationDefaults.AuthenticationScheme;
}