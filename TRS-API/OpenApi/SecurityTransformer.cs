using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using TRS_API.Middleware;

namespace TRS_API.OpenApi;

// Documents the X-API-KEY header and the bearer token, and adds them, plus a 401 response, to each
// operation that needs them: the API key unless it has [AllowWithoutApiKey], the bearer token if it
// has [Authorize]. New endpoints are covered automatically.
public class SecurityTransformer : IOpenApiDocumentTransformer, IOpenApiOperationTransformer
{
    private const string ApiKeySchemeName = "ApiKey";
    private const string BearerSchemeName = "Bearer";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[ApiKeySchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = ApiKeyMiddleware.HeaderName,
            Description = "API key required by all endpoints except those marked as public."
        };
        document.Components.SecuritySchemes[BearerSchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Access token from /user/login/verify or /user/refresh."
        };
        return Task.CompletedTask;
    }

    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        var needsApiKey = !metadata.OfType<AllowWithoutApiKeyAttribute>().Any();
        var needsBearer = metadata.OfType<IAuthorizeData>().Any() && !metadata.OfType<IAllowAnonymous>().Any();
        if (!needsApiKey && !needsBearer)
        {
            return Task.CompletedTask;
        }

        // Both schemes in one requirement means both are needed, not either
        var requirement = new OpenApiSecurityRequirement();
        if (needsApiKey)
        {
            requirement[new OpenApiSecuritySchemeReference(ApiKeySchemeName, context.Document)] = [];
        }
        if (needsBearer)
        {
            requirement[new OpenApiSecuritySchemeReference(BearerSchemeName, context.Document)] = [];
        }

        operation.Security ??= [];
        operation.Security.Add(requirement);

        var description = (needsApiKey, needsBearer) switch
        {
            (true, true) => "Missing or invalid X-API-KEY header or access token",
            (true, false) => "Missing or invalid X-API-KEY header",
            _ => "Missing or invalid access token"
        };
        operation.Responses ??= new OpenApiResponses();
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = description });
        return Task.CompletedTask;
    }
}
