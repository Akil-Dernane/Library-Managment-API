using Library_API.Data;
using Library_API.DTOs.ApplicationUser;
using Library_API.Exceptions;
using Library_API.Exceptions.Auth;
using Library_API.Exceptions.Member;
using Library_API.Exceptions.Role;
using Library_API.Exceptions.User;
using Library_API.Mappings;
using Library_API.Models;
using Library_API.Services.Admin.Interfaces;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Services.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly ICurrentUserService _currentUser;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly AppDbContext _context;


        private readonly ILogger<AdminUserService> _logger;

        public AdminUserService(
             ICurrentUserService currentUser,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            AppDbContext context,
            ILogger<AdminUserService> logger)
        {
            _currentUser = currentUser;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _logger = logger;
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

            if (!await _roleManager.RoleExistsAsync(role))
                throw new RoleNotFoundException(role);

            if (await _userManager.IsInRoleAsync(user, role))
            {
                _logger.LogInformation(
                    "User {TargetUserId} already has role {Role}. No change made by {ActorId}",
                    userId, role, actorId);

                return;
            }

            IdentityResult result =
                await _userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                var errors = result.Errors.ToList();

                _logger.LogWarning(
                    "Failed to add role {Role} to user {TargetUserId} by {ActorId}. Error codes: {ErrorCodes}",
                    userId, role, actorId,
                    string.Join(", ", errors.Select(e => e.Code)));

                if (errors.Any(e =>
                    e.Code is "DuplicateUserName" or "DuplicateEmail"))
                {
                    throw new ConflictException(
                        "An account with this username or email already exists.");
                }

                throw new RegistrationFailedException(
                    errors.Select(e => e.Description));
            }

            _logger.LogInformation(
                "User {ActorId} successfully added role {Role} to user {TargetUserId}",
                actorId, role, userId);
        }


        public async Task<IEnumerable<UserResponseDto>> GetAll(int page, int pageSize)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));

            if (pageSize < 1 || pageSize > 100)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

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
                    (user, userRoles) => new { user, userRoles }
                )
                .SelectMany(
                    x => x.userRoles.DefaultIfEmpty(),
                    (x, userRole) => new
                    {
                        x.user,
                        RoleId = userRole == null
                            ? (int?)null
                            : userRole.RoleId
                    }
                )
                .GroupJoin(
                    _context.Roles,
                    x => x.RoleId,
                    role => (int?)role.Id,
                    (x, roles) => new
                    {
                        x.user,
                        roles
                    }
                )
                .SelectMany(
                    x => x.roles.DefaultIfEmpty(),
                    (x, role) => new
                    {
                        x.user,
                        RoleName = role == null ? null : role.Name
                    }
                )
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
                        .ToList()
                ))
                .ToListAsync();

            return users;
        }


        public async Task RemoveFromRole(int userId, string role)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new UserNotFoundException(userId);

            if (!await _roleManager.RoleExistsAsync(role))
                throw new RoleNotFoundException(role);

            _logger.LogInformation(
                "User {ActorId} requested to remove role {Role} from user {UserId}",
                _currentUser.UserId, role, userId);

            if (string.Equals(role, Roles.Admin, StringComparison.OrdinalIgnoreCase))
            {
                var admins = await _userManager.GetUsersInRoleAsync(Roles.Admin);

                if (admins.Count == 1 && admins[0].Id == user.Id)
                    throw new LastAdminException();
            }

            IdentityResult result = await _userManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
            {
                _logger.LogWarning(
                    "Failed to remove role {Role} from user {UserId} by actor {ActorId}. Errors: {Errors}",
                    role,
                    userId,
                    _currentUser.UserId,
                    string.Join("; ", result.Errors.Select(e => e.Code)));

                throw new RoleRemovalFailedException(
                    result.Errors.Select(e => e.Description));
            }

            _logger.LogInformation(
                "User {ActorId} successfully removed role {Role} from user {UserId}",
                _currentUser.UserId, role, userId);
        }

    }
}
