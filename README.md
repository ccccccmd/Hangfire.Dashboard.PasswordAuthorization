# Hangfire.Dashboard.PasswordAuthorization

[![Nuget package](https://img.shields.io/nuget/vpre/UI.Hangfire.Dashboard.PasswordAuthorization)](https://www.nuget.org/packages/UI.Hangfire.Dashboard.PasswordAuthorization/)

This package allows you to protect your Hangfire dashboard with a password base on asp.net core **cookie authentication**. It is useful when you want to protect your Hangfire dashboard from unauthorized access.

## Installation

You can install the package using NuGet Package Manager:

```bash
Install-Package UI.Hangfire.Dashboard.PasswordAuthorization
```

Or using .NET CLI:

```bash
dotnet add package UI.Hangfire.Dashboard.PasswordAuthorization
```

## Usage

To enable password protection, you need to add the following code in your `Program.cs` file:

```csharp
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

service.AddAuthorization()
    .AddHangfire(conf => { conf.UseInMemoryStorage(new InMemoryStorageOptions()); })
    .AddHangfireServer(option =>
    {
        option.Queues = new[] { "sample" };
        option.ServerName = "Sample.Server";
    }).AddLogDashboard(options =>
    {
        options.Account = "admin";
        options.Password = "password";
    });


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboardWithLogin();
app.MapGet("/", () => "Hello World!");

app.Run();
```

In the example above, the password is set to `password`. You can change it to any other value.

## Preview
![preview.png](Hangfire.Dashboard.PasswordAuthorization%2FAssets%2Fpreview.png)

## Acknowledgments

- [Hangfire](https://www.hangfire.io/)
