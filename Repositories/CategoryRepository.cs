using Library_API.Data;
using Library_API.DTOs.Category;
using Library_API.Mappings;
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

        public async Task<IEnumerable<CategoryResponseDto>> GetAll()
        {
            return await _context.Categories
                .AsNoTracking()
                .Select(CategoryMapping.ToDtoExpression)
                .ToListAsync();
        }

        public async Task<Category?> GetById(int id)
        {
            return await _context.Categories
                .Include(c => c.Books)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<List<Category>> GetByIds(IEnumerable<int> Ids)
        {
            return await _context.Categories
                .Where(c => Ids.Contains(c.Id))
                .ToListAsync();
        }
        public async Task<CategoryDetailsDto?> GetByIdWithRelationships(int id)
        {
            return await _context.Categories
                .Where(c => c.Id == id)
                .Select(CategoryMapping.ToDetailsDtoExpression)
                .FirstOrDefaultAsync();
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

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
