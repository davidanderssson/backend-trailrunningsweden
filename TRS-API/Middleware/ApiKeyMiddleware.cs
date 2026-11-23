using System.Security.Cryptography;
using System.Text;

namespace TRS_API.Middleware;

public class ApiKeyMiddleware
{
    public const string HeaderName = "X-API-KEY";

    private readonly RequestDelegate _next;
    private readonly byte[] _apiKey;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        var apiKey = configuration["ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("The ApiKey setting is not configured.");
        }

        _next = next;
        _apiKey = Encoding.UTF8.GetBytes(apiKey);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<AllowWithoutApiKeyAttribute>() is not null)
        {
            await _next(context);
            return;
        }

        var providedKey = context.Request.Headers[HeaderName].ToString();
        if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(providedKey), _apiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}
