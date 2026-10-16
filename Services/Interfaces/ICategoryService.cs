using Library_API.Models;

namespace Library_API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category> GetById(int id);
        Task<Category> Create(Category category);
        Task Update(int id, Category category);
        Task Delete(int id);
    }
}
