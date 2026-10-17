using Library_API.DTOs.Author;
using Library_API.Exceptions;
using Library_API.Mappings;
using Library_API.Models;
using Library_API.Repositories.Interfaces;
using Library_API.Services.Interfaces;

namespace Library_API.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;

        public AuthorService(IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
        }

        public async Task<AuthorResponseDto> Create(CreateAuthorDto dto)
        {
            Author author = dto.ToEntity();

            foreach(int BookId in dto.BookIds)
            {
                Book book = await _bookRepository.GetById(BookId) ?? throw new BookNotFoundException(BookId);

                author.Books.Add(book);
            }

            return (await _authorRepository.Create(author)).ToDto();
        }

        public async Task Delete(int id)
        {
            bool deleted = await _authorRepository.Delete(id);

            if (!deleted)
            {
                throw new AuthorNotFoundException(id);
            }
        }

        public async Task<IEnumerable<AuthorResponseDto>> GetAll()
        {
            return await _authorRepository.GetAll();
        }

        public async Task<Author> GetById(int id)
        {
            Author author = await _authorRepository.GetById(id) ?? throw new AuthorNotFoundException(id);

            return author;
        }
        public async Task<AuthorResponseDto> GetDtoById(int id)
        {
            Author author = await _authorRepository.GetById(id) ?? throw new AuthorNotFoundException(id);

            return author.ToDto();
        }

        public async Task<AuthorDetailsDto> GetByIdWithRelationships(int id)
        {
            Author author = await _authorRepository.GetByIdWithRelationships(id) ?? throw new AuthorNotFoundException(id);
            
            return author.ToDetailsDto();
        }

        public async Task Update(int id, Author author)
        {
            bool updated = await _authorRepository.Update(id, author);

            if(!updated)
                throw new AuthorNotFoundException(id);
        }
    }
}
