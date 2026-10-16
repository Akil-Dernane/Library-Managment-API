using Library_API.Models;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers_2
{

    [ApiController]
    [Route("api/members")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;
        public MemberController(IMemberService service)
        {
            _memberService = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetAll()
        {
            return Ok(await _memberService.GetAll());
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<Member>> GetById(int id)
        {
            return Ok(await _memberService.GetById(id));
        }

        [HttpPost]
        public async Task<ActionResult<Member>> Create(Member member)
        {
            Member created = await _memberService.Create(member);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, Member member)
        {
            await _memberService.Update(id, member);
            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _memberService.Delete(id);
            return NoContent();
        }
    }
}
