using Library_API.DTOs.ApplicationUser;
using Library_API.Models;

namespace Library_API.Services.Admin.Interfaces
{
    public interface IAdminUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAll(int page, int pageSize);
        Task AddToRole(int userId, string role);
        Task RemoveFromRole(int userId, string role);
    }
}
