using System.Security.Claims;
namespace Library_API.Services.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Email { get; }
        string? MemberId { get; }
        bool IsAuthenticated { get; }
    }
}
