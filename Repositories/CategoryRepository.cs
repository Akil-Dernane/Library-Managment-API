using Library_API.Data;
using Library_API.Models;
using Library_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Category> Create(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> Delete(int id)
        {
            Category? category = await _context.Categories.FindAsync(id);
            if(category == null)
                return false;
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            return await _context.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<Category?> GetById(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public async Task<bool> Update(int id, Category updatedCategory)
        {
            Category? category = await _context.Categories.FindAsync(id);
            if (category == null)
                return false;

            category.Name = updatedCategory.Name;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
