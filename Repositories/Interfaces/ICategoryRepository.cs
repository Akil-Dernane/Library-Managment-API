using Library_API.Models;

namespace Library_API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category?> GetById(int id);
        Task<Category> Create(Category category);
        Task<bool> Update(int id, Category updatedCategory);
        Task<bool> Delete(int id);
    }
}
