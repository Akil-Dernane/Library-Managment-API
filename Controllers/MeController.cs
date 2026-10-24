using Library_API.DTOs.Loan;
using Library_API.Exceptions.Member;
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

            if (!int.TryParse(
                User.FindFirstValue(JwtRegisteredClaimNames.Sub), 
                out var applicationUserId)
                )
            {
                return Unauthorized();
            }

            try
            {
                var memberId = await _memberService
                    .GetMemberIdByUserIdAsync(applicationUserId);
                return Ok(await _loanService.GetMyLoans(memberId));
            }
            catch (MemberNotFoundException) {
                return Ok(new List<LoanResponseDto>());
            }
        }
    }
}
