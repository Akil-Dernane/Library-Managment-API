using Library_API.DTOs.Auth;

namespace Library_API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshAsync(string presentedToken);
        Task LogoutAsync(string presentedToken);
        Task LogoutAllAsync(int userId);
    }
}
