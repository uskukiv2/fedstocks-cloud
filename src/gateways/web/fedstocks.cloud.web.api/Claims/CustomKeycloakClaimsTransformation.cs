using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace fedstocks.cloud.web.api.Claims;

public class CustomKeycloakClaimsTransformation : IClaimsTransformation
{
    private readonly string _roleClaimType;
    private readonly string _audience;

    public CustomKeycloakClaimsTransformation(string roleClaimType,
        string audience)
    {
        _roleClaimType = roleClaimType;
        _audience = audience;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var result = principal.Clone();
        if (result.Identity is not ClaimsIdentity identity)
        {
            return Task.FromResult(result);
        }

        var resourceAccessValue = principal.FindFirst("resource_access")?.Value;
        if (string.IsNullOrWhiteSpace(resourceAccessValue))
        {
            return Task.FromResult(result);
        }

        using var resourceAccess = JsonDocument.Parse(resourceAccessValue);
        var containsAudienceRoles = resourceAccess
            .RootElement
            .TryGetProperty(this._audience, out var rolesElement);

        if (!containsAudienceRoles)
        {
            return Task.FromResult(result);
        }

        var clientRoles = rolesElement.GetProperty("roles");

        foreach (var value in clientRoles.EnumerateArray().Select(role => role.GetString())
                     .Where(value => !string.IsNullOrWhiteSpace(value)))
        {
            identity.AddClaim(new Claim(this._roleClaimType, value));
        }

        return Task.FromResult(result);

    }
}