using Library_API.Data;
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

        public async Task<IEnumerable<Author>> GetAll()
        {
            return await _context.Authors.AsNoTracking().ToListAsync();
        }

        public async Task<Author?> GetById(int id)
        {
            return await _context.Authors.FindAsync(id);
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
