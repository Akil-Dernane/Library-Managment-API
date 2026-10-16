using Library_API.Models;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers_2
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        public AuthorController(IAuthorService service)
        {
            _authorService = service;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAll() {
            return Ok(await _authorService.GetAll());
        }
        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<Author>> GetById(int id)
        {
            return Ok(await _authorService.GetById(id));
        }

        [HttpPost]
        public async Task<ActionResult<Author>> Create(Author author)
        {
            Author created = await _authorService.Create(author);
            return CreatedAtAction(nameof(GetById),new {id = created.Id}, author);
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, Author author) {
            await _authorService.Update(id, author);
            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _authorService.Delete(id);
            return NoContent();
        }
    }
}
