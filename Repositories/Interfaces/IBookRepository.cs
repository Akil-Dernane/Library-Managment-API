using Library_API.DTOs.Book;
using Library_API.Models;

namespace Library_API.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookResponseDto>> GetAll(int page, int pageSize);
        Task<Book?> GetById(int id);
        Task<List<Book>> GetByIds(IEnumerable<int> ids);
        Task<Book?> GetByIdWithRelationships(int id);

        Task<BookResponseDto?> GetDtoById(int id);
        Task<Book> Create(Book book);
        Task SaveChangesAsync();

        //Task<bool> Update(int id, UpdateBookDto updatedBook);
        Task<bool> Delete(int id);
    }
}
