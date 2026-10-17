using Library_API.DTOs.Category;
using Library_API.Models;

namespace Library_API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryResponseDto>> GetAll();
        Task<Category?> GetById(int id);
        Task<List<Category>> GetByIds(IEnumerable<int> ids);
        Task<CategoryDetailsDto?> GetByIdWithRelationships(int id);
        Task<Category> Create(Category category);
        Task<bool> Update(int id, Category updatedCategory);
        Task<bool> Delete(int id);
        Task SaveChangesAsync();
    }
}
