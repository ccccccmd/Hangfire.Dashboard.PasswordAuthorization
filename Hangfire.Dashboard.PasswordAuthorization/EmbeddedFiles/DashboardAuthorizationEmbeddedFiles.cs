using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace Hangfire.Dashboard.PasswordAuthorization.EmbeddedFiles;

public static class DashboardAuthorizationEmbeddedFiles
{
    private static readonly Dictionary<string, string> ResponseType = new()
    {
        { ".css", "text/css" },
        { ".js", "application/javascript" },
        { ".woff2", "font/woff2" },
        { ".html", "text/html" },
        { ".jpg", "image/jpeg" },
        { ".ttf", "application/octet-stream" }
    };

    private static readonly Assembly executingAssembly;

    static DashboardAuthorizationEmbeddedFiles()
    {
        executingAssembly = typeof(DashboardAuthorizationExtension).GetTypeInfo().Assembly;
    }

    public static async Task IncludeEmbeddedFile(HttpContext context, string path)
    {
        context.Response.OnStarting(() =>
        {
            if (context.Response.StatusCode == (int)HttpStatusCode.OK)
            {
                context.Response.ContentType = ResponseType[Path.GetExtension(path)];
            }

            return Task.CompletedTask;
        });

        try
        {
            await using var inputStream =
                executingAssembly.GetManifestResourceStream(
                    $"{typeof(DashboardAuthorizationExtension).Namespace}.Assets.{path.Substring(1).Replace('/', '.')}");
            if (inputStream == null)
            {
                throw new ArgumentException(
                    $@"Resource with name {path.Substring(1)} not found in assembly {executingAssembly}.");
            }

            await inputStream.CopyToAsync(context.Response.Body).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            await context.Response.WriteAsync($"{e.StackTrace}");
        }
    }
}