using Hangfire;
using Hangfire.Dashboard.PasswordAuthorization;
using Hangfire.InMemory;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var service = builder.Services;

service.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.Path = "/";
    });

service.AddAuthorization();
service.AddHangfire(conf => { conf.UseInMemoryStorage(new InMemoryStorageOptions()); });
service.AddHangfireServer(option =>
{
    option.Queues = new[] { "sample" };
    option.ServerName = "Sample.Server";
}).AddLogDashboard(options =>
{
    options.Account = "admin";
    options.Password = "admin";
});


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboardWithLogin();
app.MapGet("/", () => "Hello World!");

app.Run();