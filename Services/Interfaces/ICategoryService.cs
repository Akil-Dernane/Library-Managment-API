using Library_API.DTOs.Author;
using Library_API.DTOs.Category;
using Library_API.Models;

namespace Library_API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAll();
        Task<Category> GetById(int id);
        Task<CategoryResponseDto> GetDtoById(int id);
        Task<CategoryDetailsDto> GetByIdWithRelationships(int id);
        Task<CategoryResponseDto> Create(CreateCategoryDto dto);
        Task Update(int id, UpdateCategoryDto dto);
        Task Delete(int id);
    }
}
