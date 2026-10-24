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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Services.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly AppDbContext _context;

        public AdminUserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task AddToRole(int userId, string role)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new UserNotFoundException(userId);

            if(!await _roleManager.RoleExistsAsync(role))
                throw new RoleNotFoundException(role);

            if (await _userManager.IsInRoleAsync(user, role))
                return;

            IdentityResult result = await _userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                var errors = result.Errors.ToList();

                if (errors.Any(e =>
                    e.Code is "DuplicateUserName" or "DuplicateEmail"))
                {
                    throw new ConflictException(
                        "An account with this username or email already exists.");
                }

                throw new RegistrationFailedException(
                    errors.Select(e => e.Description));
            }
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

        public Task RemoveFromRole(int userId, string role)
        {
            throw new NotImplementedException();
        }
    }
}
