namespace Library_API.DTOs.Auth
{
    public record AuthResponseDto(
     string AccessToken,
     DateTime AccessTokenExpiresAtUtc,
     string RefreshToken,
     DateTime RefreshTokenExpiresAtUtc);
}
