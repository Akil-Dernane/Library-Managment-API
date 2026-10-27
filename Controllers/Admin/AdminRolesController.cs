using Library_API.Models;
using Library_API.Services.Admin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/roles")]
    [Authorize(Roles = Roles.Admin)]
    public class AdminRolesController : ControllerBase
    {
        private readonly IAdminRolesService _rolesService;
        public AdminRolesController(IAdminRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        [HttpGet("{roleName}/claims")]
        public async Task<ActionResult<IEnumerable<ClaimRequest>>> GetAllRoleClaims(string roleName)
        {
            return Ok( await _rolesService.GetAllRoleClaims(roleName));
        }

        [HttpPost("{roleName}/claims")]
        public async Task<ActionResult> AddClaimToRole(string roleName, ClaimRequest dto)
        {
            await _rolesService.AddClaimToRole(roleName, dto);
            return NoContent();
        }

        [HttpDelete("{roleName}/claims")]
        public async Task<IActionResult> RemoveClaimFromRole(string roleName, ClaimRequest dto)
        {
            await _rolesService.RemoveClaimFromRole(roleName, dto);   
            return NoContent();
        }
    }
}
