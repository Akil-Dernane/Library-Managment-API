namespace Library_API.DTOs.ApplicationUser
{
    public record UserResponseDto(
    int Id,
    string? UserName,
    string? Email,
    bool EmailConfirmed,
    string? PhoneNumber,
    bool PhoneNumberConfirmed,
    bool TwoFactorEnabled,
    DateTime CreatedAtUtc,
    IReadOnlyList<string> Roles
);
}
