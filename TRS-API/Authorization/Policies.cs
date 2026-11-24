namespace TRS_API.Authorization;

// Names of the authorization policies, for use as [Authorize(Policy = Policies.Admin)]
public static class Policies
{
    // The logged in user must have Admin = true
    public const string Admin = "Admin";
}
