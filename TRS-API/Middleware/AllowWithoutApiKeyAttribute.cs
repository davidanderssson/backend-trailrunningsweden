namespace TRS_API.Middleware;

// Marks controllers, actions or endpoints that can be called without an X-API-KEY header
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowWithoutApiKeyAttribute : Attribute
{
}
