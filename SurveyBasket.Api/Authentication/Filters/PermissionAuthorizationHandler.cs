using Microsoft.AspNetCore.Authorization;
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Authentication.Filters;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var user = context.User.Identity;

        if (user is null || !user.IsAuthenticated)
            return Task.CompletedTask;

        var hasPermission = context.User.Claims.Any(x => x.Value == requirement.Permission && x.Type == Permissions.Type);

        if (hasPermission)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
