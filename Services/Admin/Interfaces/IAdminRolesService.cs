using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Library_API.Services.Admin.Interfaces
{
    public interface IAdminRolesService
    {
        Task<IEnumerable<Claim>> GetAllRoleClaims(string roleName);
        Task AddClaimToRole(string roleName, ClaimRequest dto);
        Task RemoveClaimFromRole(string roleName, ClaimRequest dto);
    }
}
