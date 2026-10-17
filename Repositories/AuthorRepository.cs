using Library_API.Data;
using Library_API.DTOs.Author;
using Library_API.Mappings;
using Library_API.Models;
using Library_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Repositories
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly AppDbContext _context;
        public AuthorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Author> Create(Author author)
        {
            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<bool> Delete(int id)
        {
            Author? author = await _context.Authors.FindAsync(id);
            if (author == null) { 
                return false;
            }
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AuthorResponseDto>> GetAll()
        {
            return await _context.Authors
                .AsNoTracking()
                .Select(AuthorMapping.ToDtoExpression)
                .ToListAsync();
        }

        public async Task<Author?> GetById(int id)
        {
            return await _context.Authors.FindAsync(id);
        }
        public async Task<List<Author>> GetByIds(IEnumerable<int> Ids)
        {
            return await _context.Authors
                .Where(a => Ids.Contains(a.Id))
                .ToListAsync();
        }
        public async Task<Author?> GetByIdWithRelationships(int id)
        {
            return await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<AuthorResponseDto?> GetDtoById(int id)
        {
            return await _context.Authors
                .Where(a => a.Id == id)
                .Select(AuthorMapping.ToDtoExpression)
                .FirstOrDefaultAsync();

        }

        public async Task<bool> Update(int id, Author updatedAuthor)
        {
            Author? author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return false;
            }

            author.Name = updatedAuthor.Name;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
