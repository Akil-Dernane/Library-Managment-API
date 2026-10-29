using Library_API.DTOs.Auth;
using Library_API.Models;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Library_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
           _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting(Policies.Auth)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            return Ok(await _authService.LoginAsync(dto));
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            RegisterResponseDto response = await _authService.RegisterAsync(dto);

            return CreatedAtAction(
                nameof(Register),
                new { id = response.Id },
                response
            );
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [EnableRateLimiting(Policies.Auth)]
        public async Task<ActionResult<AuthResponseDto>> Refresh(RefreshRequestDto dto)
        {
            return Ok(await _authService.RefreshAsync(dto.RefreshToken));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshRequestDto dto)
        {
            await _authService.LogoutAsync(dto.RefreshToken);
            return NoContent();
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAll()
        {
            if (!int.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out int userId))
                return Unauthorized();

            await _authService.LogoutAllAsync(userId);

            return NoContent();
        }
    }
}
