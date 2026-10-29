
using Library_API.Data;
using Library_API.DTOs.Auth;
using Library_API.Exceptions;
using Library_API.Exceptions.Auth;
using Library_API.Models;
using Library_API.Repositories.Interfaces;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using static Library_API.Models.AppPermissions;

namespace Library_API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    private readonly ITokenService _tokens;
    private readonly JwtSettings _jwt;

    private readonly IMemberRepository _memberRepository;

    private readonly AppDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        SignInManager<ApplicationUser> signInManager, 
        ITokenService tokens, 
        IOptions<JwtSettings> jwt,
        IMemberRepository memberRepository,
         AppDbContext context
        )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _tokens = tokens;
        _jwt = jwt.Value;
        _memberRepository = memberRepository;
        _context = context;
    }



    public async Task<RegisterResponseDto> RegisterAsync(RegisterDto dto)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(
                user,
                dto.Password
            );

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
                    result.Errors.Select(e => e.Description)
                );
            }

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                Roles.Member
            );

            if (!roleResult.Succeeded)
            {
                throw new RegistrationFailedException(
                    roleResult.Errors.Select(e => e.Description)
                );
            }

            var member = new Member
            {
                Name = dto.Name,
                DateOfBirth = dto.DateOfBirth!.Value,
                UserId = user.Id
            };

            await _memberRepository.Create(member);

            await transaction.CommitAsync();

            return new RegisterResponseDto(user.Id, user.Email!);
        }
        catch
        {
            try
            {
                await transaction.RollbackAsync();
            }
            catch (Exception rollbackException)
            {
                // Use your application's ILogger<AuthService>.
                // Log rollbackException without replacing the original error.
            }

            throw;
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        ApplicationUser user = await _userManager.FindByEmailAsync(dto.Email) ?? throw new InvalidCredentialsException();

        SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
            throw new AccountLockedException(await _userManager.GetLockoutEndDateAsync(user));

        if (!result.Succeeded)
            throw new InvalidCredentialsException();

        var (entity,raw) = NewRefreshToken(user.Id,Guid.NewGuid());
        _context.RefreshTokens.Add(entity);
        await _context.SaveChangesAsync();

        var (access, accessExpires) = await BuildAccessTokenAsync(user);

        return new AuthResponseDto(access, accessExpires, raw, entity.ExpiresAtUtc);
    }

    private async Task<(string Token , DateTime ExpiresAtUtc)> BuildAccessTokenAsync(ApplicationUser user)
    {
        var claims = new List<Claim>(await _userManager.GetClaimsAsync(user));

        int memberId = await _memberRepository.GetMemberIdByUserIdAsync(user.Id);

        claims.Add(new Claim(AppClaims.MemberId, memberId.ToString()));

        foreach (string roleName in await _userManager.GetRolesAsync(user))
        {
            claims.Add(new Claim("role", roleName));

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is not null)
                claims.AddRange(await _roleManager.GetClaimsAsync(role));
        }

        IList<string> roles = await _userManager.GetRolesAsync(user);
        string token = _tokens.CreateAccessToken(user.Id, user.Email!, claims.DistinctBy(c => (c.Type, c.Value)));

        return (token, DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes));
    }

    private (RefreshToken Entity, string raw) NewRefreshToken(int userId,Guid familyId)
    {
        string raw = RefreshTokenGenerator.Generate();
        var now = DateTime.UtcNow;

        var entity = new RefreshToken
        {
            UserId = userId,
            TokenHash = RefreshTokenGenerator.Hash(raw),
            FamilyId = familyId,
            CreatedAtUtc = now,
            ExpiresAtUtc = now.AddDays(_jwt.RefreshTokenDays),
        };
        return (entity, raw);
    }

    public async Task<AuthResponseDto> RefreshAsync(string presentedToken)
    {
        string hash = RefreshTokenGenerator.Hash(presentedToken);
        DateTime now = DateTime.UtcNow;

        RefreshToken stored = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hash) ?? throw new InvalidRefreshTokenException();

        if(stored.RevokedAtUtc is not null)
        {
            await RevokeFamilyAsync(stored.FamilyId, now);
            throw new InvalidRefreshTokenException();
        }

        if(stored.ExpiresAtUtc <= now || await _userManager.IsLockedOutAsync(stored.User))
        {
            throw new InvalidRefreshTokenException();
        }

        var (next, raw) = NewRefreshToken(stored.UserId, stored.FamilyId);

        await using var tx = await _context.Database.BeginTransactionAsync();

        int claimed = await _context.RefreshTokens
            .Where(t => t.Id == stored.Id && t.RevokedAtUtc == null)
            .ExecuteUpdateAsync(s => s
            .SetProperty(t => t.RevokedAtUtc, now)
            .SetProperty(t => t.ReplacedByTokenHash, next.TokenHash));

        if (claimed == 0)                                         
        {
            await tx.RollbackAsync();
            await RevokeFamilyAsync(stored.FamilyId, now);
            throw new InvalidRefreshTokenException();
        }

        _context.RefreshTokens.Add(next);
        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        var (access, accessExpires) = await BuildAccessTokenAsync(stored.User);
        return new AuthResponseDto(access, accessExpires, raw, next.ExpiresAtUtc);
    }

    private async Task RevokeFamilyAsync(Guid familyId, DateTime now)
    {
        await _context.RefreshTokens
       .Where(t => t.FamilyId == familyId && t.RevokedAtUtc == null)
       .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAtUtc, now));
    }

    public async Task LogoutAsync(string presentedToken)
    {
        string hash = RefreshTokenGenerator.Hash(presentedToken);
        await _context.RefreshTokens
            .Where(t => t.TokenHash == hash && t.RevokedAtUtc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAtUtc, DateTime.UtcNow));
    }
    public async Task LogoutAllAsync(int userId)
    {
        await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAtUtc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAtUtc, DateTime.UtcNow));
    }
}
