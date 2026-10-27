
using Library_API.Exceptions.Claim;
using Library_API.Exceptions.Permission;
using Library_API.Exceptions.Role;
using Library_API.Models;
using Library_API.Services.Admin.Interfaces;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Library_API.Services.Admin;

public class AdminRolesService : IAdminRolesService
{
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<AdminRolesService> _logger;

    public AdminRolesService(
        RoleManager<IdentityRole<int>> roleManager,
        ICurrentUserService currentUser,
        ILogger<AdminRolesService> logger)
    {
        _roleManager = roleManager;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task AddClaimToRole(string roleName, ClaimRequest dto)
    {
        _logger.LogInformation(
            "User {ActorId} requested to add claim {ClaimType} - {ClaimValue} to role {RoleName}",
            _currentUser.UserId, dto.Type, dto.Value, roleName);

        ValidateRoleClaim(dto);

        var role = await FindRoleAsync(roleName);

        var claim = new Claim(dto.Type, dto.Value);
        var existingClaims = await _roleManager.GetClaimsAsync(role);

        if (existingClaims.Any(c =>
            c.Type == claim.Type && c.Value == claim.Value))
        {
            _logger.LogInformation(
                "Claim {ClaimType} - {ClaimValue} already exists on role {RoleName}; no changes made",
                dto.Type, dto.Value, roleName);

            return;
        }

        var result = await _roleManager.AddClaimAsync(role, claim);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Failed to add claim {ClaimType} - {ClaimValue} to role {RoleName}. Errors: {Errors}",
                dto.Type,
                dto.Value,
                roleName,
                string.Join(", ", result.Errors.Select(e => e.Code)));

            throw new RoleClaimAddFailedException(
                result.Errors.Select(e => e.Description));
        }

        _logger.LogInformation(
            "User {ActorId} successfully added claim {ClaimType} - {ClaimValue} to role {RoleName}",
            _currentUser.UserId, dto.Type, dto.Value, roleName);
    }

    public async Task<IEnumerable<Claim>> GetAllRoleClaims(string roleName)
    {
        var role = await FindRoleAsync(roleName);
        var claims = await _roleManager.GetClaimsAsync(role);

        return claims;
    }

    public async Task RemoveClaimFromRole(string roleName, ClaimRequest dto)
    {
        _logger.LogInformation(
            "User {ActorId} requested to remove claim {ClaimType} - {ClaimValue} from role {RoleName}",
            _currentUser.UserId, dto.Type, dto.Value, roleName);

        ValidateRoleClaim(dto);

        var role = await FindRoleAsync(roleName);

        var claim = new Claim(dto.Type, dto.Value);
        var existingClaims = await _roleManager.GetClaimsAsync(role);

        if (!existingClaims.Any(c =>
            c.Type == claim.Type && c.Value == claim.Value))
        {
            _logger.LogInformation(
                "Claim {ClaimType} - {ClaimValue} does not exist on role {RoleName} no changes made",
                dto.Type, dto.Value, roleName);

            return;
        }

        var result = await _roleManager.RemoveClaimAsync(role, claim);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Failed to remove claim {ClaimType} - {ClaimValue} from role {RoleName}. Errors: {Errors}",
                dto.Type,
                dto.Value,
                roleName,
                string.Join(", ", result.Errors.Select(e => e.Code)));

            throw new RoleClaimRemoveFailedException(
                result.Errors.Select(e => e.Description));
        }

        _logger.LogInformation(
            "User {ActorId} successfully removed claim {ClaimType} - {ClaimValue} from role {RoleName}",
            _currentUser.UserId, dto.Type, dto.Value, roleName);
    }

    private async Task<IdentityRole<int>> FindRoleAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);

        if (role is null)
        {
            _logger.LogWarning(
                "Role {RoleName} was not found",
                roleName);

            throw new RoleNotFoundException(roleName);
        }

        return role;
    }

    private void ValidateRoleClaim(ClaimRequest dto)
    {
        if (!AppClaims.RoleClaimTypes.Contains(dto.Type))
        {
            _logger.LogWarning(
                "User {ActorId} attempted to use invalid claim type {ClaimType}",
                _currentUser.UserId,
                dto.Type);

            throw new InvalidClaimException(dto.Type);
        }

        switch (dto.Type)
        {
            case AppClaims.Permission:
                if (!AppPermissions.All.Contains(dto.Value))
                {
                    _logger.LogWarning(
                        "User {ActorId} attempted to use invalid permission {Permission}",
                        _currentUser.UserId,
                        dto.Value);

                    throw new InvalidPermissionException(dto.Value);
                }

                break;
        }
    }
}
