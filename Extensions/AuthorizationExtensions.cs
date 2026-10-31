using Library_API.Authorization;
using Library_API.Models;
using Microsoft.AspNetCore.Authorization;

namespace Library_API.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddLibraryAuthorization(
        this IServiceCollection services)
    {
        var authorization = services.AddAuthorizationBuilder()
            .AddPolicy(Policies.StaffOnly, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireRole(Roles.Admin, Roles.Librarian))

            .AddPolicy(Policies.CanBorrow, policy =>
                policy.RequireAuthenticatedUser()
                      .AddRequirements(new MinimumAgeRequirement(14)))

            .SetFallbackPolicy(
                new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build());

        foreach (var permission in AppPermissions.All)
        {
            authorization.AddPolicy(permission, policy =>
                policy.RequireClaim(AppClaims.Permission, permission));
        }

        return services;
    }
}