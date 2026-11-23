using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using TRS_API.Middleware;

namespace TRS_API.OpenApi;

// Documents the X-API-KEY header and adds it, plus a 401 response, to every operation
// that is not marked with [AllowWithoutApiKey]. New endpoints are covered automatically.
public class ApiKeyTransformer : IOpenApiDocumentTransformer, IOpenApiOperationTransformer
{
    private const string SchemeName = "ApiKey";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = ApiKeyMiddleware.HeaderName,
            Description = "API key required by all endpoints except those marked as public."
        };
        return Task.CompletedTask;
    }

    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var isPublic = context.Description.ActionDescriptor.EndpointMetadata.OfType<AllowWithoutApiKeyAttribute>().Any();
        if (isPublic)
        {
            return Task.CompletedTask;
        }

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SchemeName, context.Document)] = []
        });

        operation.Responses ??= new OpenApiResponses();
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Missing or invalid X-API-KEY header" });
        return Task.CompletedTask;
    }
}
