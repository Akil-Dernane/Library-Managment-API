using Library_API.Exceptions;
using Library_API.Models;
using Library_API.Repositories.Interfaces;
using Library_API.Services.Interfaces;

namespace Library_API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category> Create(Category category)
        {
            return await _categoryRepository.Create(category);
        }

        public async Task Delete(int id)
        {
            bool deleted = await _categoryRepository.Delete(id);
            if (!deleted)
                throw new CategoryNotFoundException(id);
        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            return await _categoryRepository.GetAll();
        }

        public async Task<Category> GetById(int id)
        {
            Category? category = await _categoryRepository.GetById(id);

            if(category == null)
                throw new CategoryNotFoundException(id);

            return category;
        }

        public async Task Update(int id, Category category)
        {
            bool updated = await _categoryRepository.Update(id, category);

            if(!updated)
                throw new CategoryNotFoundException(id);
        }
    }
}
