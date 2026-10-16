using Library_API.Models;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers_2
{
    [ApiController]
    [Route("api/loans")]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;
        public LoanController(ILoanService service)
        {
            _loanService = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Loan>>> GetAll()
        {
            return Ok(await _loanService.GetAll());
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<Loan>> GetById(int id)
        {
            return Ok(await _loanService.GetById(id));
        }

        [HttpPost]
        public async Task<ActionResult<Loan>> Create(Loan loan)
        {
            Loan created = await _loanService.Create(loan);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, Loan loan)
        {
            await _loanService.Update(id, loan);
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
