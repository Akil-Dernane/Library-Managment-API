using Library_API.DTOs.ApplicationUser;
using Library_API.Models;
using Library_API.Services.Admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = Roles.Admin)]
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


        [HttpPost("{userId:int:min(1)}/roles")]
        public async Task<IActionResult> AddRole(int userId, AssignRoleDto dto)
        {
            await _usersService.AddToRole(userId, dto.Role);
            return NoContent();
        }

        //[HttpDelete("{userId:int:min(1)}/roles/{role}")]
        //public async Task<IActionResult> RemoveRole(int userId, string role)
        //{
        //    await _service.RemoveFromRole(userId, role);
        //    return NoContent();
        //}
    }
}
