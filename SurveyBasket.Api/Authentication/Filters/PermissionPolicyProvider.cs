using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SurveyBasket.Api.Authentication.Filters;

public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options):DefaultAuthorizationPolicyProvider(options)
{
    private readonly AuthorizationOptions _options = options.Value;

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);

        if (policy != null)
            return policy;

        var policyPermission = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionRequirement(policyName))
            .Build();

        _options.AddPolicy(policyName, policyPermission);

        return policyPermission;
    }
}
