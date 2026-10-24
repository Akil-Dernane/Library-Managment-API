using Library_API.DTOs.Category;
using Library_API.Models;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers_2
{
    [ApiController]
    [Route("api/categories")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService service)
        {
            _categoryService = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAll()
        {
            return Ok(await _categoryService.GetAll());
        }

        [HttpGet("{id:int:min(1)}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryResponseDto>> GetDtoById(int id)
        {
            return Ok(await _categoryService.GetDtoById(id));
        }

        [HttpGet("{id:int:min(1)}/details")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryDetailsDto>> GetByIdWithRelationships(int id)
        {
            return Ok(await _categoryService.GetByIdWithRelationships(id));
        }

        [HttpPost]
        [Authorize(Roles = Roles.AdminOrLibrarian)]
        public async Task<ActionResult<CategoryResponseDto>> Create(CreateCategoryDto dto)
        {
            CategoryResponseDto created = await _categoryService.Create(dto);

            return CreatedAtAction(
                nameof(GetDtoById),
                new { id = created.Id },
                created);
        }

        [HttpPut("{id:int:min(1)}")]
        [Authorize(Roles = Roles.AdminOrLibrarian)]
        public async Task<IActionResult> Update(int id, UpdateCategoryDto dto)
        {
            await _categoryService.Update(id, dto);
            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.Delete(id);
            return NoContent();
        }
    }
}
