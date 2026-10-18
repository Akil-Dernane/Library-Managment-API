using Library_API.DTOs.Author;
using Library_API.DTOs.Book;
using Library_API.Models;

namespace Library_API.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorResponseDto>> GetAll();
        Task<Author> GetById(int id);
        Task<AuthorResponseDto> GetDtoById(int id);
        Task<AuthorDetailsDto> GetByIdWithRelationships(int id);
        Task<AuthorResponseDto> Create(CreateAuthorDto dto);
        Task Update(int id, UpdateAuthorDto dto);
        Task Delete(int id);
    }
}
