using Library_API.DTOs.Author;
using Library_API.DTOs.Category;
using Library_API.Models;

namespace Library_API.DTOs.Book
{
    public record BookDetailsDto(
        BookResponseDto Book,
        IReadOnlyList<AuthorResponseDto> Authors,
        IReadOnlyList<CategoryResponseDto> Categories
        );
}
