using Library_API.Authorization;
using Library_API.DTOs.ApplicationUser;
using Library_API.Models;
using Library_API.Services.Admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library_API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    [HasPermission(AppPermissions.Users.Manage)]
    public class UsersController : ControllerBase
    {
        private readonly IAdminUserService _usersService;

        public UsersController(IAdminUserService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll(int page = 1,int? pageSize = null)
        {
            int size = pageSize ?? 25;
            return Ok(await _usersService.GetAll(page, size));
        }

        #region Manage User Roles
        [HttpPost("{userId:int:min(1)}/roles")]
        public async Task<IActionResult> AddRole(int userId, AssignRoleDto dto)
        {
            await _usersService.AddToRole(userId, dto.Role);
            return NoContent();
        }

        [HttpDelete("{userId:int:min(1)}/roles/{role}")]
        public async Task<IActionResult> RemoveRole(int userId, string role)
        {
            await _usersService.RemoveFromRole(userId, role);
            return NoContent();
        }
        #endregion

        #region Manage User Claims
        [HttpGet("{userId:int:min(1)}/claims")]
        public async Task<ActionResult<IEnumerable<Claim>>> GetAllUserClaims(int userId)
        {
            return Ok(await _usersService.GetAllUserClaims(userId));
        }

        [HttpPost("{userId:int:min(1)}/claims")]
        public async Task<IActionResult> AddClaimToUser(int userId, ClaimRequest dto)
        {
            await _usersService.AddClaimToUser(userId, dto);
            return NoContent();
        }

        [HttpDelete("{userId:int:min(1)}/claims")]
        public async Task<IActionResult> RemoveClaimFromUser(int userId, ClaimRequest dto)
        {
            await _usersService.RemoveClaimFromUser(userId, dto);
            return NoContent();
        }

        #endregion
    }
}
