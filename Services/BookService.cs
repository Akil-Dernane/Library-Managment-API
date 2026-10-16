using Library_API.DTOs.Book;
using Library_API.Exceptions;
using Library_API.Mappings;
using Library_API.Models;
using Library_API.Repositories.Interfaces;
using Library_API.Services.Interfaces;

namespace Library_API.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly ICategoryRepository _categoryRepository;
        public BookService(
            IBookRepository bookRepository, 
            IAuthorRepository authorRepository,
            ICategoryRepository categoryRepository
            )
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;

        }

        public async Task<BookResponseDto> Create(CreateBookDto dto)
        {
            Book book = dto.ToEntity();

            // Adding the Authors
            foreach(int authorId in dto.AuthorIds)
            {
                var author = await _authorRepository.GetById(authorId);
                if(author == null)
                    throw new AuthorNotFoundException(authorId);
                book.Authors.Add(author);
            }

            // Adding the Categories
            foreach(int CategoryId in dto.CategoryIds)
            {
                var category = await _categoryRepository.GetById(CategoryId);
                if(category == null)
                    throw new CategoryNotFoundException(CategoryId);
                book.Categories.Add(category);
            }

            return (await _bookRepository.Create(book)).ToDto();
        }

        public async Task Delete(int id)
        {
            bool deleted = await _bookRepository.Delete(id);
            if (!deleted)
                throw new BookNotFoundException(id);
        }

        public async Task<IEnumerable<BookResponseDto>> GetAll(int page, int pageSize)
        {
            if (page < 1)
                throw new ArgumentOutOfRangeException(nameof(page));

            if (pageSize is < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            return await _bookRepository.GetAll(page, pageSize);
        }

        public async Task<Book> GetById(int id)
        {
            Book? book = await _bookRepository.GetById(id);
            if(book == null)
                throw new BookNotFoundException(id);
            return book;
        }
        public async Task<BookResponseDto> GetDotById(int id)
        {
            Book? book = await _bookRepository.GetById(id);
            if(book == null)
                throw new BookNotFoundException(id);
            return book.ToDto();
        }

        public async Task<BookDetailsDto> GetByIdWithRelationships(int id)
        {
            Book? book = await _bookRepository.GetByIdWithRelationships(id);
            if (book == null)
                throw new BookNotFoundException(id);

            return book.ToDetailsDto();
        }

        public async Task Update(int id, UpdateBookDto dto)
        {
            Book? book = await _bookRepository.GetByIdWithRelationships(id);

            if (book is null)
                throw new BookNotFoundException(id);

            if (dto.AuthorIds is not null)
            {
                book.Authors.Clear();
                foreach (var authorId in dto.AuthorIds)
                {
                    Author? auther = await _authorRepository.GetById(authorId);

                    if (auther is null)
                        throw new AuthorNotFoundException(authorId);

                    book.Authors.Add(auther);
                }
            }

            if (dto.CategoryIds is not null)
            {
                book.Categories.Clear();
                foreach (var categoryId in dto.CategoryIds)
                {
                    Category? category = await _categoryRepository.GetById(categoryId);

                    if(category is null)
                        throw new CategoryNotFoundException(categoryId);

                    book.Categories.Add(category);
                }
            }
            dto.UpdateEntity(book);
            await _bookRepository.SaveChangesAsync();
        }
    }
}
