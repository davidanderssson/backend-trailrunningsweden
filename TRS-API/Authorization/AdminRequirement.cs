using Microsoft.AspNetCore.Authorization;
using TRS_API.Extensions;
using TRS_Lib.Repositories;

namespace TRS_API.Authorization;

public class AdminRequirement : IAuthorizationRequirement
{
}

// Looks the user up on every request instead of trusting a claim in the token,
// so removing someone's admin rights takes effect immediately
public class AdminHandler(IUserRepository userRepository) : AuthorizationHandler<AdminRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
    {
        if (context.User.GetUserId() is not { } userId)
        {
            return;
        }

        var user = await userRepository.FindAsync(userId);
        if (user is { Admin: true })
        {
            context.Succeed(requirement);
        }
    }
}
