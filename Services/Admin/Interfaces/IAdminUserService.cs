using Library_API.DTOs.ApplicationUser;
using System.Security.Claims;

namespace Library_API.Services.Admin.Interfaces
{
    public interface IAdminUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAll(int page, int pageSize);
        Task AddToRole(int userId, string role);
        Task RemoveFromRole(int userId, string role);

        Task<IEnumerable<Claim>> GetAllUserClaims(int userId);
        Task AddClaimToUser(int userId,ClaimRequest dto);
        Task RemoveClaimFromUser(int userId,ClaimRequest dto);
    }
}
