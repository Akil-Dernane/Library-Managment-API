using Library_API.DTOs.Author;
using Library_API.Exceptions;
using Library_API.Exceptions.Book;
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
            var bookIds = dto.BookIds.Distinct().ToList();

            var books = bookIds.Count == 0
                ? new List<Book>()
                : await _bookRepository.GetByIds(bookIds);

            var foundIds = books.Select(b => b.Id).ToHashSet();

            var missingId = bookIds.FirstOrDefault(id => !foundIds.Contains(id));

            if (bookIds.Any(id => !foundIds.Contains(id)))
                throw new BookNotFoundException(missingId);

            Author author= dto.ToEntity();

            author.Books.AddRange(books);

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

        public async Task Update(int id, UpdateAuthorDto dto)
        {
            Author author = await _authorRepository.GetByIdWithRelationships(id) ?? throw new AuthorNotFoundException(id);
            author.UpdateEntity(dto);

            if (dto.BookIds is not null)
            {
                HashSet<int> requestedIds = dto.BookIds.ToHashSet();
                HashSet<int> existingIds = author.Books
                    .Select(b => b.Id)
                    .ToHashSet();


                if (!requestedIds.SetEquals(existingIds))
                {
                    List<int> idsToAdd = requestedIds
                        .Except(existingIds)
                        .ToList();

                    List<int> idsToRemove = existingIds
                        .Except(requestedIds)
                        .ToList();

                    // Fetch only books that need to be added
                    List<Book> booksToAdd = idsToAdd.Count == 0
                        ? new List<Book>()
                        : await _bookRepository.GetByIds(idsToAdd);

                    HashSet<int> foundIds = booksToAdd.Select(b => b.Id).ToHashSet();

                    int? missingId = idsToAdd
                        .Where(bookId => !foundIds.Contains(bookId))
                        .Select(bookId => (int?)bookId)
                        .FirstOrDefault();

                    if (missingId.HasValue)
                        throw new BookNotFoundException(missingId.Value);

                    foreach (Book book in author.Books
                       .Where(b => idsToRemove.Contains(b.Id))
                       .ToList())
                    {
                        author.Books.Remove(book);
                    }

                    author.Books.AddRange(booksToAdd);
                }
            }
            await _authorRepository.SaveChangesAsync();
        }
    }
}
