using Library_API.DTOs.Author;
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
        public async Task<ActionResult<IEnumerable<AuthorResponseDto>>> GetAll() {
            return Ok(await _authorService.GetAll());
        }
        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<AuthorResponseDto>> GetById(int id)
        {
            return Ok(await _authorService.GetDtoById(id));
        }
        [HttpGet("{id:int:min(1)}/details")]
        public async Task<ActionResult<AuthorDetailsDto>> GetByIdWithRelationships(int id)
        {
            return Ok(await _authorService.GetByIdWithRelationships(id));
        }

        [HttpPost]
        public async Task<ActionResult<AuthorResponseDto>> Create(CreateAuthorDto dto)
        {
            AuthorResponseDto created = await _authorService.Create(dto);
            return CreatedAtAction(nameof(GetById),new {id = created.Id}, created);
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, UpdateAuthorDto dto) {
            await _authorService.Update(id, dto);
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
