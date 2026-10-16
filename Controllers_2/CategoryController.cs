using Library_API.Models;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Controllers_2
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService service)
        {
            _categoryService = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            return Ok(await _categoryService.GetAll());
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            return Ok(await _categoryService.GetById(id));
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Create(Category category)
        {
            Category created = await _categoryService.Create(category);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
        }

        [HttpPut("{id:int:min(1)}")]
        public async Task<IActionResult> Update(int id, Category category)
        {
            await _categoryService.Update(id, category);
            return NoContent();
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.Delete(id);
            return NoContent();
        }
    }
}
