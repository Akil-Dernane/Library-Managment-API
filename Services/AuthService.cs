
using Library_API.Auditing;
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
    private readonly IAuditService _audit;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        SignInManager<ApplicationUser> signInManager, 
        ITokenService tokens, 
        IOptions<JwtSettings> jwt,
        IMemberRepository memberRepository,
         AppDbContext context,
         IAuditService audit
        )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _tokens = tokens;
        _jwt = jwt.Value;
        _memberRepository = memberRepository;
        _context = context;
        _audit = audit;
    }



    public async Task<RegisterResponseDto> RegisterAsync(RegisterDto dto)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            if (result.Errors.Any(e =>
                e.Code is "DuplicateUserName" or "DuplicateEmail"))
            {
                throw new ConflictException(
                    "An account with this username or email already exists.");
            }

            throw new RegistrationFailedException(
                result.Errors.Select(e => e.Description));
        }

        var roleResult = await _userManager.AddToRoleAsync(
            user, Roles.Member);

        if (!roleResult.Succeeded)
        {
            throw new RegistrationFailedException(
                roleResult.Errors.Select(e => e.Description));
        }

        var member = new Member
        {
            Name = dto.Name,
            DateOfBirth = dto.DateOfBirth!.Value,
            UserId = user.Id
        };

        await _memberRepository.Create(member);

        _audit.Record(new AuditEntry(
            AuditEvents.Registered,
            AuditOutcome.Success,
            ActorUserId: user.Id,
            ActorIdentifier: user.Email?.Trim().ToUpperInvariant(),
            TargetType: "User",
            TargetId: user.Id.ToString()
        ));

        await transaction.CommitAsync();

        return new RegisterResponseDto(user.Id, user.Email!);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(dto.Email);

        if(user is null)
        {
            await _audit.WriteAsync(new AuditEntry 
            (AuditEvents.LoginFailed, 
                AuditOutcome.Failure,
                ActorIdentifier: dto.Email.Trim().ToUpperInvariant(),
                Details: new {reason = "unknown_email"}));
            throw new InvalidCredentialsException();
        }


        SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            await _audit.WriteAsync(new AuditEntry(
                AuditEvents.LoginLockedOut, AuditOutcome.Denied,
                ActorUserId: user.Id,
                ActorIdentifier: user.Email?.ToUpperInvariant(),
                Details: new {reason ="account_locked_out"}));
            throw new AccountLockedException(await _userManager.GetLockoutEndDateAsync(user));
        }

        if (!result.Succeeded)
        {
            await _audit.WriteAsync(new AuditEntry(
                AuditEvents.LoginFailed, AuditOutcome.Failure,
                ActorUserId: user.Id,
                ActorIdentifier: user.Email?.ToUpperInvariant(),
                Details: new {reason = "bad_password"}));
            throw new InvalidCredentialsException();
        }

        await _audit.WriteAsync(new AuditEntry(
            AuditEvents.LoginSuccess,AuditOutcome.Success, 
            ActorUserId:user.Id, ActorIdentifier: user.Email?.ToUpperInvariant()));

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
            .FirstOrDefaultAsync(t => t.TokenHash == hash)
            ?? throw new InvalidRefreshTokenException();

        string? actorIdentifier =
            stored.User.Email?.Trim().ToUpperInvariant();

        // 1. Check whether the token was already revoked.
        if (stored.RevokedAtUtc is not null)
        {
            if (stored.ReplacedByTokenHash is not null)
            {
                await RevokeFamilyAsync(stored.FamilyId, now);

                await _audit.WriteAsync(new AuditEntry(
                    AuditEvents.RefreshReuseDetected,
                    AuditOutcome.Denied,
                    ActorUserId: stored.UserId,
                    ActorIdentifier: actorIdentifier,
                    TargetType: "RefreshTokenFamily",
                    TargetId: stored.FamilyId.ToString()
                ));
            }

            throw new InvalidRefreshTokenException();
        }

        // 2. Check expiration.
        if (stored.ExpiresAtUtc <= now)
        {
            await _audit.WriteAsync(new AuditEntry(
                AuditEvents.RefreshTokenExpired,
                AuditOutcome.Failure,
                ActorUserId: stored.UserId,
                ActorIdentifier: actorIdentifier,
                TargetType: "RefreshToken",
                TargetId: stored.Id.ToString()
            ));

            throw new InvalidRefreshTokenException();
        }

        // 3. Check account lockout.
        if (await _userManager.IsLockedOutAsync(stored.User))
        {
            await _audit.WriteAsync(new AuditEntry(
                AuditEvents.RefreshTokenDenied,
                AuditOutcome.Denied,
                ActorUserId: stored.UserId,
                ActorIdentifier: actorIdentifier,
                TargetType: "RefreshToken",
                TargetId: stored.Id.ToString()
            ));

            throw new InvalidRefreshTokenException();
        }

        // 4. Generate the replacement refresh token.
        var (next, raw) = NewRefreshToken(
            stored.UserId,
            stored.FamilyId
        );

        // 5. Atomically claim the existing token.
        await using var tx =
            await _context.Database.BeginTransactionAsync();

        int claimed = await _context.RefreshTokens
            .Where(t => t.Id == stored.Id && t.RevokedAtUtc == null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.RevokedAtUtc, now)
                .SetProperty(t => t.ReplacedByTokenHash, next.TokenHash));

        // 6. Another request may have rotated this token first.
        if (claimed == 0)
        {
            await tx.RollbackAsync();

            // Re-read the latest state to distinguish actual reuse
            // from a token revoked for another reason.
            var latest = await _context.RefreshTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == stored.Id);

            if (latest?.ReplacedByTokenHash is not null)
            {
                await RevokeFamilyAsync(stored.FamilyId, now);

                await _audit.WriteAsync(new AuditEntry(
                    AuditEvents.RefreshReuseDetected,
                    AuditOutcome.Denied,
                    ActorUserId: stored.UserId,
                    ActorIdentifier: actorIdentifier,
                    TargetType: "RefreshTokenFamily",
                    TargetId: stored.FamilyId.ToString()
                ));
            }

            throw new InvalidRefreshTokenException();
        }

        // 7. Save the replacement token.
        _context.RefreshTokens.Add(next);

        // 8. Record successful refresh in the same transaction.
        _audit.Record(new AuditEntry(
            AuditEvents.TokenRefreshed,
            AuditOutcome.Success,
            ActorUserId: stored.UserId,
            ActorIdentifier: actorIdentifier,
            TargetType: "RefreshToken",
            TargetId: next.Id.ToString()
        ));

        await _context.SaveChangesAsync();

        // 9. Generate access token before committing.
        var (access, accessExpires) =
            await BuildAccessTokenAsync(stored.User);

        // 10. Commit rotation and audit record together.
        await tx.CommitAsync();

        return new AuthResponseDto(
            access,
            accessExpires,
            raw,
            next.ExpiresAtUtc
        );
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
        DateTime now = DateTime.UtcNow;

        var stored = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hash);

        if (stored is null || stored.RevokedAtUtc is not null)
            return;

        int revoked = await _context.RefreshTokens
            .Where(t => t.Id == stored.Id && t.RevokedAtUtc == null)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(t => t.RevokedAtUtc, now));

        if (revoked == 0)
            return;

        await _audit.WriteAsync(new AuditEntry(
            AuditEvents.Logout,
            AuditOutcome.Success,
            ActorUserId: stored.UserId,
            ActorIdentifier: stored.User.Email?.Trim().ToUpperInvariant(),
            TargetType: "RefreshToken",
            TargetId: stored.Id.ToString()
        ));
    }
    public async Task LogoutAllAsync(int userId)
    {
        DateTime now = DateTime.UtcNow;

        int revoked = await _context.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAtUtc == null)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(t => t.RevokedAtUtc, now));

        if (revoked == 0)
            return;

        var user = await _userManager.FindByIdAsync(userId.ToString());

        await _audit.WriteAsync(new AuditEntry(
            AuditEvents.LogoutAll,
            AuditOutcome.Success,
            ActorUserId: userId,
            ActorIdentifier: user?.Email?.Trim().ToUpperInvariant(),
            TargetType: "User",
            TargetId: userId.ToString(),
            Details: new { revokedTokenCount = revoked }
        ));
    }
}
