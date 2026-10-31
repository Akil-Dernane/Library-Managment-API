using Library_API.Auditing;
using Library_API.Data;
using Library_API.Exceptions.Role;
using Library_API.Models;
using Library_API.Services.Admin.Interfaces;
using Library_API.Services.Interfaces;
using Library_API.Services.Validation.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Library_API.Services.Admin;

public class AdminRolesService : IAdminRolesService
{
    private readonly AppDbContext _context;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<AdminRolesService> _logger;
    private readonly IClaimValidationService _claimValidation;
    private readonly IAuditService _audit;

    public AdminRolesService(
        AppDbContext context,
        RoleManager<IdentityRole<int>> roleManager,
        ICurrentUserService currentUser,
        ILogger<AdminRolesService> logger,
        IClaimValidationService claimValidation,
        IAuditService audit)
    {
        _context = context;
        _roleManager = roleManager;
        _currentUser = currentUser;
        _logger = logger;
        _claimValidation = claimValidation;
        _audit = audit;
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

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

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

        _audit.Record(new AuditEntry(
            EventType: AuditEvents.RoleClaimAdded,
            Outcome: AuditOutcome.Success,
            ActorUserId: GetActorUserId(),
            ActorIdentifier: GetActorIdentifier(),
            TargetType: "Role",
            TargetId: role.Id.ToString(),
            Details: new
            {
                roleName = role.Name,
                claimType = dto.Type,
                claimValue = dto.Value
            }
        ));

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        _logger.LogInformation(
            "User {ActorId} successfully added claim {ClaimType} - {ClaimValue} to role {RoleName}",
            _currentUser.UserId, dto.Type, dto.Value, roleName);
    }

    public async Task<IEnumerable<Claim>> GetAllRoleClaims(string roleName)
    {
        var role = await FindRoleAsync(roleName);

        return await _roleManager.GetClaimsAsync(role);

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
                "Claim {ClaimType} - {ClaimValue} does not exist on role {RoleName}; no changes made",
                dto.Type, dto.Value, roleName);

            return;
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

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

        _audit.Record(new AuditEntry(
            EventType: AuditEvents.RoleClaimRemoved,
            Outcome: AuditOutcome.Success,
            ActorUserId: GetActorUserId(),
            ActorIdentifier: GetActorIdentifier(),
            TargetType: "Role",
            TargetId: role.Id.ToString(),
            Details: new
            {
                roleName = role.Name,
                claimType = dto.Type,
                claimValue = dto.Value
            }
        ));

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

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

    private int? GetActorUserId()
    {
        return int.TryParse(_currentUser.UserId, out var userId)
            ? userId
            : null;
    }

    private string? GetActorIdentifier()
    {
        return _currentUser.Email?.Trim().ToUpperInvariant();
    }
}