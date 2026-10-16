using Library_API.Models;

namespace Library_API.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<Author>> GetAll();
        Task<Author> GetById(int id);
        Task<Author> Create(Author author);
        Task Update(int id,Author author);
        Task Delete(int id);
    }
}
