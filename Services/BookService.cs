using Library_API.DTOs.Book;
using Library_API.Exceptions;
using Library_API.Exceptions.Author;
using Library_API.Exceptions.Book;
using Library_API.Exceptions.Category;
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

            // Add Authors
            List<int> authorIds = dto.AuthorIds.Distinct().ToList();
            List<Author> authors = await _authorRepository.GetByIds(authorIds);

            HashSet<int> foundAuthorIds = authors.Select(a => a.Id).ToHashSet();

            int? missingAuthorId = authorIds
                .Where(id => !foundAuthorIds.Contains(id))
                .Select(id => (int?)id)
                .FirstOrDefault();

            if (missingAuthorId.HasValue)
                throw new AuthorNotFoundException(missingAuthorId.Value);

            book.Authors.AddRange(authors);

            // Add Categories
            List<int> categoryIds = dto.CategoryIds.Distinct().ToList();
            List<Category> categories = await _categoryRepository.GetByIds(categoryIds);

            HashSet<int> foundCategoryIds = categories.Select(c => c.Id).ToHashSet();

            int? missingCategoryId = categoryIds
                .Where(id => !foundCategoryIds.Contains(id))
                .Select(id => (int?)id)
                .FirstOrDefault();

            if (missingCategoryId.HasValue)
                throw new CategoryNotFoundException(missingCategoryId.Value);

            book.Categories.AddRange(categories);

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
            Book book = await _bookRepository.GetById(id) ?? throw new BookNotFoundException(id);

            return book;
        }
        public async Task<BookResponseDto> GetDotById(int id)
        {
            Book book = await _bookRepository.GetById(id) ?? throw new BookNotFoundException(id);

            return book.ToDto();
        }

        public async Task<BookDetailsDto> GetByIdWithRelationships(int id)
        {
            Book book = await _bookRepository.GetByIdWithRelationships(id) ?? throw new BookNotFoundException(id);

            return book.ToDetailsDto();
        }
        public async Task Update(int id, UpdateBookDto dto)
        {
            Book book = await _bookRepository.GetByIdWithRelationships(id)
                ?? throw new BookNotFoundException(id);

            if (dto.AuthorIds is not null)
            {
                HashSet<int> requestedIds = dto.AuthorIds.ToHashSet();
                HashSet<int> existingIds = book.Authors.Select(a => a.Id).ToHashSet();

                if (!requestedIds.SetEquals(existingIds))
                {
                    HashSet<int> idsToAdd = requestedIds.Except(existingIds).ToHashSet();
                    HashSet<int> idsToRemove = existingIds.Except(requestedIds).ToHashSet();

                    List<Author> authorsToAdd = idsToAdd.Count == 0
                        ? new List<Author>()
                        : await _authorRepository.GetByIds(idsToAdd.ToList());

                    HashSet<int> foundIds = authorsToAdd.Select(a => a.Id).ToHashSet();

                    int? missingId = idsToAdd
                        .Where(authorId => !foundIds.Contains(authorId))
                        .Select(authorId => (int?)authorId)
                        .FirstOrDefault();

                    if (missingId.HasValue)
                        throw new AuthorNotFoundException(missingId.Value);

                    foreach (Author author in book.Authors
                        .Where(a => idsToRemove.Contains(a.Id))
                        .ToList())
                    {
                        book.Authors.Remove(author);
                    }

                    foreach (Author author in authorsToAdd)
                    {
                        book.Authors.Add(author);
                    }
                }
            }

            if (dto.CategoryIds is not null)
            {
                HashSet<int> requestedIds = dto.CategoryIds.ToHashSet();
                HashSet<int> existingIds = book.Categories.Select(c => c.Id).ToHashSet();

                if (!requestedIds.SetEquals(existingIds))
                {
                    HashSet<int> idsToAdd = requestedIds.Except(existingIds).ToHashSet();
                    HashSet<int> idsToRemove = existingIds.Except(requestedIds).ToHashSet();

                    List<Category> categoriesToAdd = idsToAdd.Count == 0
                        ? new List<Category>()
                        : await _categoryRepository.GetByIds(idsToAdd.ToList());

                    HashSet<int> foundIds = categoriesToAdd.Select(c => c.Id).ToHashSet();

                    int? missingId = idsToAdd
                        .Where(categoryId => !foundIds.Contains(categoryId))
                        .Select(id => (int?)id)
                        .FirstOrDefault();

                    if (missingId.HasValue)
                        throw new CategoryNotFoundException(missingId.Value);

                    foreach (Category category in book.Categories
                        .Where(c => idsToRemove.Contains(c.Id))
                        .ToList())
                    {
                        book.Categories.Remove(category);
                    }

                    foreach (Category category in categoriesToAdd)
                    {
                        book.Categories.Add(category);
                    }
                }
            }

            dto.UpdateEntity(book);

            await _bookRepository.SaveChangesAsync();
        }
    }
}
