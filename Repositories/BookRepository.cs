using Library_API.Data;
using Library_API.DTOs.Book;
using Library_API.Mappings;
using Library_API.Models;
using Library_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        public BookRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Book> Create(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> Delete(int id)
        {
            Book? book = await _context.Books.FindAsync(id);
            if (book == null)
                return false;
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BookResponseDto>> GetAll(int page, int pageSize)
        {
            return await _context.Books.AsNoTracking()
                .OrderBy(b => b.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(BookMapping.ToDtoExpression)
                .ToArrayAsync();
        }
        public async Task<Book?> GetById(int id)
        {
            return await _context.Books.FindAsync(id);
        }
        public async Task<Book?> GetByIdWithRelationships(int id)
        {
            return await _context.Books
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                 .AsSplitQuery()
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        public async Task<BookResponseDto?> GetDtoById(int id)
        {
            return await _context.Books
                .Where(b => b.Id == id)
                .Select(b => new BookResponseDto(
                    b.Id,
                    b.Name,
                    b.Pages,
                    b.IsAvailable))
                .FirstOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        //public async Task<bool> Update(int id, UpdateBookDto dto)
        //{
        //    Book? book = await _context.Books.FindAsync(id);
        //    if (book == null)
        //        return false;

        //    dto.UpdateEntity(book);

        //    await _context.SaveChangesAsync();
        //    return true;
        //}
    }
}
