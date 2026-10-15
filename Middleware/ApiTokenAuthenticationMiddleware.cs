using System.Security.Cryptography;
using System.Text;

namespace UserManagementAPI.Middleware;

public sealed class ApiTokenAuthenticationMiddleware(
    RequestDelegate next,
    IConfiguration configuration)
{
    private const string TokenConfigurationKey = "Authentication:BearerToken";

    public async Task InvokeAsync(HttpContext context)
    {
        var configuredToken = configuration[TokenConfigurationKey];
        if (string.IsNullOrWhiteSpace(configuredToken))
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Authentication is not configured."
            });
            return;
        }

        var authorizationHeader = context.Request.Headers.Authorization.ToString();
        const string bearerPrefix = "Bearer ";
        var suppliedToken = authorizationHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase)
            ? authorizationHeader[bearerPrefix.Length..].Trim()
            : string.Empty;

        if (!TokensMatch(configuredToken, suppliedToken))
        {
            context.Response.Headers.WWWAuthenticate = "Bearer";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "A valid bearer token is required."
            });
            return;
        }

        await next(context);
    }

    private static bool TokensMatch(string expected, string supplied)
    {
        var expectedBytes = Encoding.UTF8.GetBytes(expected);
        var suppliedBytes = Encoding.UTF8.GetBytes(supplied);

        return expectedBytes.Length == suppliedBytes.Length &&
               CryptographicOperations.FixedTimeEquals(expectedBytes, suppliedBytes);
    }
}
