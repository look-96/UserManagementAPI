using System.Diagnostics;

namespace UserManagementAPI.Middleware;

public sealed class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        context.Response.OnCompleted(() =>
        {
            stopwatch.Stop();
            logger.LogInformation(
                "HTTP {Method} {Path} returned {StatusCode} in {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.Elapsed.TotalMilliseconds);
            return Task.CompletedTask;
        });

        await next(context);
    }
}
