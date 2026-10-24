using Library_API.DTOs.Book;
using Library_API.Models;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Library_API.Controllers_2
{

    [ApiController]
    [Authorize]
    [Route("api/books")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ApiSettings _apiSettings;
        public BookController(
            IBookService service,
            IOptions<ApiSettings> apiSettings
            )
        {
            _bookService = service;
            _apiSettings = apiSettings.Value;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BookResponseDto>>> GetAll(int page = 1, int? pageSize = null)
        {
            int size = Math.Min(pageSize?? _apiSettings.DefaultPageSize, _apiSettings.MaxPageSize);
            return Ok(await _bookService.GetAll(page,size));
        }

        [HttpGet("{id:int:min(1)}")]
        [AllowAnonymous]
        public async Task<ActionResult<BookResponseDto>> GetById(int id)
        {
            return Ok( await _bookService.GetDotById(id));
        }

        [HttpGet("{id:int:min(1)}/details")]
        [Authorize(Roles = Roles.AdminOrLibrarian)]
        public async Task<ActionResult<BookDetailsDto>> GetBookDetails(int id)
        {
            return Ok(await _bookService.GetByIdWithRelationships(id));
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdminOrLibrarian)]
        public async Task<ActionResult<BookResponseDto>> Create(CreateBookDto dto)
        {
            BookResponseDto created = await _bookService.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int:min(1)}")]
        [Authorize(Roles = Roles.AdminOrLibrarian)]
        public async Task<IActionResult> Update(int id, UpdateBookDto dto)
        {
            await _bookService.Update(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> Delete(int id) {
            await _bookService.Delete(id);
            return NoContent();
        }
    }
}
