using Library_API.DTOs.Book;
using Library_API.Models;

namespace Library_API.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookResponseDto>> GetAll(int page, int pageSize);
        Task<Book> GetById(int id);
        Task<BookResponseDto> GetDotById(int id);
        Task<BookDetailsDto> GetByIdWithRelationships(int id);
        Task<BookResponseDto> Create(CreateBookDto dto);
        Task Update(int id, UpdateBookDto dto);
        Task Delete(int id);
    }
}
