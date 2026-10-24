namespace Library_API.DTOs.Auth
{
    public record AuthResponseDto(string AccessToken, DateTime ExpiresAtUtc);
}
