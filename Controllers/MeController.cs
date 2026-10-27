using Library_API.DTOs.Loan;
using Library_API.Exceptions.Member;
using Library_API.Models;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Library_API.Controllers
{
    [ApiController]
    [Route("api/me")]
    [Authorize]
    public class MeController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly IMemberService _memberService;

        public MeController(ILoanService loanService, IMemberService memberService)
        {
            _loanService = loanService;
            _memberService = memberService;
        }


        [HttpGet]
        public IActionResult Me()
        {
            string? userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            string? email = User.FindFirstValue(JwtRegisteredClaimNames.Email);
            string? role = User.FindFirst("role")?.Value;

            return Ok(new { userId, email, role });
        }

        [HttpGet("loans")]
        public async Task<ActionResult<IEnumerable<LoanResponseDto>>> GetMyLoans()
        {
            var memberId = int.Parse(User.FindFirstValue(AppClaims.MemberId));
            return Ok(await _loanService.GetMyLoans(memberId));
        }
    }
}
