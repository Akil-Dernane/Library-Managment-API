using System.Security.Claims;
namespace Library_API.Services.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        bool IsAuthenticated { get; }
    }
}
