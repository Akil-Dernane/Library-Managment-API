using Library_API.Auditing;
using Library_API.Data;
using Library_API.DTOs.ApplicationUser;
using Library_API.Exceptions;
using Library_API.Exceptions.Auth;
using Library_API.Exceptions.Role;
using Library_API.Exceptions.User;
using Library_API.Models;
using Library_API.Services.Admin.Interfaces;
using Library_API.Services.Interfaces;
using Library_API.Services.Validation.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Library_API.Services.Admin;

public class AdminUserService : IAdminUserService
{
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly AppDbContext _context;
    private readonly ILogger<AdminUserService> _logger;
    private readonly IClaimValidationService _claimValidation;
    private readonly IAuditService _audit;

    public AdminUserService(
        ICurrentUserService currentUser,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        AppDbContext context,
        ILogger<AdminUserService> logger,
        IClaimValidationService claimValidation,
        IAuditService audit)
    {
        _currentUser = currentUser;
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _logger = logger;
        _claimValidation = claimValidation;
        _audit = audit;
    }

    public async Task AddClaimToUser(int userId, ClaimRequest dto)
    {
        var actorId = _currentUser.UserId;

        _logger.LogInformation(
            "User {ActorId} requested to add claim {ClaimType} to user {TargetUserId}",
            actorId, dto.Type, userId);

        _claimValidation.Validate(dto, AppClaims.UserClaimTypes);

        ApplicationUser user =
            await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new UserNotFoundException(userId);

        var existingClaims = await _userManager.GetClaimsAsync(user);
        var claim = new Claim(dto.Type, dto.Value);

        if (existingClaims.Any(c =>
            c.Type == claim.Type && c.Value == claim.Value))
        {
            _logger.LogInformation(
                "User {TargetUserId} already has claim {ClaimType}. No change made by {ActorId}",
                userId, dto.Type, actorId);

            return;
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        IdentityResult result = await _userManager.AddClaimAsync(user, claim);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Failed to add claim {ClaimType} to user {TargetUserId} by {ActorId}. Error codes: {ErrorCodes}",
                dto.Type,
                userId,
                actorId,
                string.Join(", ", result.Errors.Select(e => e.Code)));

            throw new UserClaimAddFailedException(
                result.Errors.Select(e => e.Description));
        }

        _audit.Record(new AuditEntry(
            EventType: AuditEvents.UserClaimAdded,
            Outcome: AuditOutcome.Success,
            ActorUserId: GetActorUserId(),
            ActorIdentifier: GetActorIdentifier(),
            TargetType: "User",
            TargetId: user.Id.ToString(),
            Details: new
            {
                claimType = dto.Type,
                claimValue = dto.Value
            }
        ));

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        _logger.LogInformation(
            "User {ActorId} successfully added claim {ClaimType} to user {TargetUserId}",
            actorId, dto.Type, userId);
    }

    public async Task AddToRole(int userId, string role)
    {
        var actorId = _currentUser.UserId;

        _logger.LogInformation(
            "User {ActorId} requested to add role {Role} to user {TargetUserId}",
            actorId, role, userId);

        ApplicationUser user =
            await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new UserNotFoundException(userId);

        var roleEntity = await _roleManager.FindByNameAsync(role);

        if (roleEntity is null)
        {
            _logger.LogWarning(
                "User {ActorId} attempted to assign nonexistent role {Role} to user {TargetUserId}",
                actorId, role, userId);

            throw new RoleNotFoundException(role);
        }

        if (await _userManager.IsInRoleAsync(user, role))
        {
            _logger.LogInformation(
                "User {TargetUserId} already has role {Role}. No change made by {ActorId}",
                userId, role, actorId);

            return;
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        IdentityResult result = await _userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
        {
            var errors = result.Errors.ToList();

            _logger.LogWarning(
                "Failed to add role {Role} to user {TargetUserId} by {ActorId}. Error codes: {ErrorCodes}",
                role,
                userId,
                actorId,
                string.Join(", ", errors.Select(e => e.Code)));

            throw new RegistrationFailedException(
                errors.Select(e => e.Description));
        }

        _audit.Record(new AuditEntry(
            EventType: AuditEvents.RoleAssigned,
            Outcome: AuditOutcome.Success,
            ActorUserId: GetActorUserId(),
            ActorIdentifier: GetActorIdentifier(),
            TargetType: "User",
            TargetId: user.Id.ToString(),
            Details: new
            {
                roleId = roleEntity.Id,
                roleName = roleEntity.Name
            }
        ));

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        _logger.LogInformation(
            "User {ActorId} successfully added role {Role} to user {TargetUserId}",
            actorId, role, userId);
    }

    public async Task<IEnumerable<UserResponseDto>> GetAll(int page, int pageSize)
    {
        _logger.LogInformation(
            "User {ActorId} requested users page {Page} with page size {PageSize}",
            _currentUser.UserId, page, pageSize);

        if (page < 1)
        {
            _logger.LogWarning(
                "User {ActorId} supplied invalid page number {Page}",
                _currentUser.UserId, page);

            throw new ArgumentOutOfRangeException(nameof(page));
        }

        if (pageSize < 1 || pageSize > 100)
        {
            _logger.LogWarning(
                "User {ActorId} supplied invalid page size {PageSize}",
                _currentUser.UserId, pageSize);

            throw new ArgumentOutOfRangeException(nameof(pageSize));
        }

        var pagedUsers = _context.Users
            .AsNoTracking()
            .OrderBy(user => user.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var users = await pagedUsers
            .GroupJoin(
                _context.UserRoles,
                user => user.Id,
                userRole => userRole.UserId,
                (user, userRoles) => new { user, userRoles })
            .SelectMany(
                x => x.userRoles.DefaultIfEmpty(),
                (x, userRole) => new
                {
                    x.user,
                    RoleId = userRole == null
                        ? (int?)null
                        : userRole.RoleId
                })
            .GroupJoin(
                _context.Roles,
                x => x.RoleId,
                role => (int?)role.Id,
                (x, roles) => new { x.user, roles })
            .SelectMany(
                x => x.roles.DefaultIfEmpty(),
                (x, role) => new
                {
                    x.user,
                    RoleName = role == null ? null : role.Name
                })
            .GroupBy(x => x.user)
            .Select(g => new UserResponseDto(
                g.Key.Id,
                g.Key.UserName,
                g.Key.Email,
                g.Key.EmailConfirmed,
                g.Key.PhoneNumber,
                g.Key.PhoneNumberConfirmed,
                g.Key.TwoFactorEnabled,
                g.Key.CreatedAtUtc,
                g.Where(x => x.RoleName != null)
                    .Select(x => x.RoleName!)
                    .ToList()))
            .ToListAsync();

        _logger.LogInformation(
            "User {ActorId} retrieved {UserCount} users for page {Page}",
            _currentUser.UserId, users.Count, page);

        return users;
    }

    public async Task<IEnumerable<Claim>> GetAllUserClaims(int userId)
    {
        _logger.LogInformation(
            "User {ActorId} requested claims for user {TargetUserId}",
            _currentUser.UserId, userId);

        ApplicationUser user =
            await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new UserNotFoundException(userId);

        var claims = await _userManager.GetClaimsAsync(user);

        _logger.LogInformation(
            "Retrieved {ClaimCount} claims for user {TargetUserId}",
            claims.Count, userId);

        return claims;
    }

    public async Task RemoveClaimFromUser(int userId, ClaimRequest dto)
    {
        var actorId = _currentUser.UserId;

        _logger.LogInformation(
            "User {ActorId} requested to remove claim {ClaimType} from user {TargetUserId}",
            actorId, dto.Type, userId);

        _claimValidation.Validate(dto, AppClaims.UserClaimTypes);

        ApplicationUser user =
            await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new UserNotFoundException(userId);

        var existingClaims = await _userManager.GetClaimsAsync(user);
        var claim = new Claim(dto.Type, dto.Value);

        if (!existingClaims.Any(c =>
            c.Type == claim.Type && c.Value == claim.Value))
        {
            _logger.LogInformation(
                "User {TargetUserId} does not have claim {ClaimType}. No change made by {ActorId}",
                userId, dto.Type, actorId);

            return;
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        IdentityResult result = await _userManager.RemoveClaimAsync(user, claim);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Failed to remove claim {ClaimType} from user {TargetUserId} by {ActorId}. Error codes: {ErrorCodes}",
                dto.Type,
                userId,
                actorId,
                string.Join(", ", result.Errors.Select(e => e.Code)));

            throw new UserClaimRemoveFailedException(
                result.Errors.Select(e => e.Description));
        }

        _audit.Record(new AuditEntry(
            EventType: AuditEvents.UserClaimRemoved,
            Outcome: AuditOutcome.Success,
            ActorUserId: GetActorUserId(),
            ActorIdentifier: GetActorIdentifier(),
            TargetType: "User",
            TargetId: user.Id.ToString(),
            Details: new
            {
                claimType = dto.Type,
                claimValue = dto.Value
            }
        ));

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        _logger.LogInformation(
            "User {ActorId} successfully removed claim {ClaimType} from user {TargetUserId}",
            actorId, dto.Type, userId);
    }

    public async Task RemoveFromRole(int userId, string role)
    {
        var actorId = _currentUser.UserId;

        _logger.LogInformation(
            "User {ActorId} requested to remove role {Role} from user {TargetUserId}",
            actorId, role, userId);

        ApplicationUser user =
            await _userManager.FindByIdAsync(userId.ToString())
            ?? throw new UserNotFoundException(userId);

        var roleEntity = await _roleManager.FindByNameAsync(role);

        if (roleEntity is null)
        {
            _logger.LogWarning(
                "User {ActorId} attempted to remove nonexistent role {Role} from user {TargetUserId}",
                actorId, role, userId);

            throw new RoleNotFoundException(role);
        }

        if (!await _userManager.IsInRoleAsync(user, role))
        {
            _logger.LogInformation(
                "User {TargetUserId} does not have role {Role}. No change made by {ActorId}",
                userId, role, actorId);

            return;
        }

        if (string.Equals(role, Roles.Admin, StringComparison.OrdinalIgnoreCase))
        {
            var admins = await _userManager.GetUsersInRoleAsync(Roles.Admin);

            if (admins.Count == 1 && admins[0].Id == user.Id)
            {
                _logger.LogWarning(
                    "User {ActorId} attempted to remove the last Admin role from user {TargetUserId}",
                    actorId, userId);

                throw new LastAdminException();
            }
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        IdentityResult result = await _userManager.RemoveFromRoleAsync(user, role);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Failed to remove role {Role} from user {TargetUserId} by {ActorId}. Error codes: {ErrorCodes}",
                role,
                userId,
                actorId,
                string.Join(", ", result.Errors.Select(e => e.Code)));

            throw new RoleRemovalFailedException(
                result.Errors.Select(e => e.Description));
        }

        _audit.Record(new AuditEntry(
            EventType: AuditEvents.RoleRemoved,
            Outcome: AuditOutcome.Success,
            ActorUserId: GetActorUserId(),
            ActorIdentifier: GetActorIdentifier(),
            TargetType: "User",
            TargetId: user.Id.ToString(),
            Details: new
            {
                roleId = roleEntity.Id,
                roleName = roleEntity.Name
            }
        ));

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        _logger.LogInformation(
            "User {ActorId} successfully removed role {Role} from user {TargetUserId}",
            actorId, role, userId);
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