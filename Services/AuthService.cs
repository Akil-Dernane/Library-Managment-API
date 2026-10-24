
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Library_API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    private readonly ITokenService _tokens;
    private readonly JwtSettings _jwt;

    private readonly IMemberRepository _memberRepository;

    private readonly AppDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, 
        ITokenService tokens, 
        IOptions<JwtSettings> jwt,
        IMemberRepository memberRepository,
         AppDbContext context
        )
    {
        _userManager = userManager;
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

        IList<string> roles = await _userManager.GetRolesAsync(user);
        string token = _tokens.CreateAccessToken(user.Id, user.Email!, roles);

        return new AuthResponseDto(token, DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes));
    }
}
