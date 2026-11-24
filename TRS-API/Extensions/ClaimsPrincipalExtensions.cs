using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace TRS_API.Extensions;

public static class ClaimsPrincipalExtensions
{
    // The logged in user's id from the access token's "sub" claim, or null if it is missing or not a number
    public static int? GetUserId(this ClaimsPrincipal principal)
    {
        return int.TryParse(principal.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId) ? userId : null;
    }
}
