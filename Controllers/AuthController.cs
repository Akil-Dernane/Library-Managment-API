using Library_API.DTOs.Auth;
using Library_API.Models;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
    }
}
