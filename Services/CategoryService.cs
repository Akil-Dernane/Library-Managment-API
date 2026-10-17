using Library_API.DTOs.Author;
using Library_API.DTOs.Category;
using Library_API.Exceptions;
using Library_API.Mappings;
using Library_API.Models;
using Library_API.Repositories.Interfaces;
using Library_API.Services.Interfaces;

namespace Library_API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBookRepository _bookRepository;

        public CategoryService(ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;

        }

        public async Task<CategoryResponseDto> Create(CreateCategoryDto dto)
        {
            var bookIds = dto.BookIds.Distinct().ToList();

            var books = bookIds.Count == 0
                ? new List<Book>()
                : await _bookRepository.GetByIds(bookIds);

            var foundIds = books.Select(b => b.Id).ToHashSet();

            var missingId = bookIds.FirstOrDefault(id => !foundIds.Contains(id));

            if (bookIds.Any(id => !foundIds.Contains(id)))
                throw new BookNotFoundException(missingId);

            Category category = dto.ToEntity();

            category.Books.AddRange(books);

            return (await _categoryRepository.Create(category)).ToDto();
        }

        public async Task Delete(int id)
        {
            bool deleted = await _categoryRepository.Delete(id);
            if (!deleted)
                throw new CategoryNotFoundException(id);
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAll()
        {
            return await _categoryRepository.GetAll();
        }

        public async Task<Category> GetById(int id)
        {
            Category category = await _categoryRepository.GetById(id) ?? throw new CategoryNotFoundException(id);

            return category;
        }
        public async Task<CategoryResponseDto> GetDtoById(int id)
        {
            Category category = await _categoryRepository.GetById(id) ?? throw new CategoryNotFoundException(id);

            return category.ToDto();
        }

        public async Task<CategoryDetailsDto> GetByIdWithRelationships(int id)
        {
            return await _categoryRepository.GetByIdWithRelationships(id) ?? throw new CategoryNotFoundException(id);

        }
        public async Task Update(int id, UpdateCategoryDto dto)
        {
            Category category = await _categoryRepository.GetById(id)
                ?? throw new CategoryNotFoundException(id);

            category.UpdateEntity(dto);

            if (dto.BookIds is not null)
            {
                HashSet<int> requestedIds = dto.BookIds.ToHashSet();
                HashSet<int> existingIds = category.Books
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

                    foreach (Book book in category.Books
                        .Where(b => idsToRemove.Contains(b.Id))
                        .ToList())
                    {
                        category.Books.Remove(book);
                    }

                    category.Books.AddRange(booksToAdd);
                }
            }

            await _categoryRepository.SaveChangesAsync();
        }
    }
}
