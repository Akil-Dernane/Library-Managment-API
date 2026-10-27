
using Library_API.Exceptions.Claim;
using Library_API.Exceptions.Role;
using Library_API.Models;
using Library_API.Services.Admin.Interfaces;
using Library_API.Services.Interfaces;
using Library_API.Services.Validation.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Library_API.Services.Admin;

public class AdminRolesService : IAdminRolesService
{
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<AdminRolesService> _logger;

    private readonly IClaimValidationService _claimValidation;

    public AdminRolesService(
        RoleManager<IdentityRole<int>> roleManager,
        ICurrentUserService currentUser,
        ILogger<AdminRolesService> logger,
        IClaimValidationService claimValidation)
    {
        _roleManager = roleManager;
        _currentUser = currentUser;
        _logger = logger;
        _claimValidation = claimValidation;
    }

    public async Task AddClaimToRole(string roleName, ClaimRequest dto)
    {
        _logger.LogInformation(
            "User {ActorId} requested to add claim {ClaimType} - {ClaimValue} to role {RoleName}",
            _currentUser.UserId, dto.Type, dto.Value, roleName);

        _claimValidation.Validate(dto, AppClaims.RoleClaimTypes);

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

        _claimValidation.Validate(dto, AppClaims.RoleClaimTypes);

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
}
