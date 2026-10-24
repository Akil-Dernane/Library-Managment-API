using Library_API.DTOs.Loan;
using Library_API.Models;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers_2
{
    [ApiController]
    [Authorize(Roles = Roles.AdminOrLibrarian)]
    [Route("api/loans")]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;
        public LoanController(ILoanService service)
        {
            _loanService = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanResponseDto>>> GetAll()
        {
            return Ok(await _loanService.GetAll());
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<LoanResponseDto>> GetById(int id)
        {
            return Ok(await _loanService.GetById(id));
        }

        [HttpPost]
        public async Task<ActionResult<LoanResponseDto>> Create(CreateLoanDto dto)
        {
            LoanResponseDto created = await _loanService.Create(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
        }

        [HttpPost("{id:int:min(1)}/return")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            await _loanService.ReturnBook(id);
            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _loanService.Delete(id);
            return NoContent();
        }
    }
}
