using Library_API.Exceptions;
using Library_API.Models;
using Library_API.Repositories.Interfaces;
using Library_API.Services.Interfaces;

namespace Library_API.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<Author> Create(Author author)
        {
            return await _authorRepository.Create(author);
        }

        public async Task Delete(int id)
        {
            bool deleted = await _authorRepository.Delete(id);

            if (!deleted)
            {
                throw new AuthorNotFoundException(id);
            }
        }

        public async Task<IEnumerable<Author>> GetAll()
        {
            return await _authorRepository.GetAll();
        }

        public async Task<Author> GetById(int id)
        {
            Author? author = await _authorRepository.GetById(id);

            if (author == null)
                throw new AuthorNotFoundException(id);

            return author;
        }

        public async Task Update(int id, Author author)
        {
            bool updated = await _authorRepository.Update(id, author);

            if(!updated)
                throw new AuthorNotFoundException(id);
        }
    }
}
