using Library_API.DTOs.Author;
using Library_API.Models;

namespace Library_API.Repositories.Interfaces
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<AuthorResponseDto>> GetAll();
        Task<Author?> GetById(int id);
        Task<List<Author>> GetByIds(IEnumerable<int> ids);
        Task<Author?> GetByIdWithRelationships(int id);
        Task<AuthorResponseDto?> GetDtoById(int id);
        Task<Author> Create(Author author);
        Task<bool> Update(int id, Author updatedAuthor);
        Task<bool> Delete(int id);
    }
}
