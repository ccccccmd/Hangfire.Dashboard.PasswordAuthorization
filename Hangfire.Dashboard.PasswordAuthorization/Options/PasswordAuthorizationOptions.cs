using Microsoft.AspNetCore.Authentication.Cookies;

namespace Hangfire.Dashboard.PasswordAuthorization.Options;

public class PasswordAuthorizationOptions
{
    public string? Account { get; set; }
    public string? Password { get; set; }
}