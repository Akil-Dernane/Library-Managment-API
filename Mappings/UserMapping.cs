
using Library_API.DTOs.ApplicationUser;
using Library_API.Models;
using System.Linq.Expressions;

namespace Library_API.Mappings;

public static class UserMapping
{
    public static Expression<Func<ApplicationUser, UserResponseDto>>
        ToDtoExpression => user => new UserResponseDto(
            user.Id,
            user.UserName,
            user.Email,
            user.EmailConfirmed,
            user.PhoneNumber,
            user.PhoneNumberConfirmed,
            user.TwoFactorEnabled,
            user.CreatedAtUtc,
            Array.Empty<string>()
        );

    public static UserResponseDto ToDto(
        this ApplicationUser user,
        IReadOnlyList<string>? roles = null)
    {
        return new UserResponseDto(
            user.Id,
            user.UserName,
            user.Email,
            user.EmailConfirmed,
            user.PhoneNumber,
            user.PhoneNumberConfirmed,
            user.TwoFactorEnabled,
            user.CreatedAtUtc,
            roles ?? Array.Empty<string>()
        );
    }
}
