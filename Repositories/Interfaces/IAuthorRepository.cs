using Library_API.Models;

namespace Library_API.Repositories.Interfaces
{
    public interface IAuthorRepository
    {
        Task<IEnumerable<Author>> GetAll();
        Task<Author?> GetById(int id);
        Task<Author> Create(Author author);
        Task<bool> Update(int id, Author updatedAuthor);
        Task<bool> Delete(int id);
    }
}
