using Library_API.DTOs.Book;

namespace Library_API.DTOs.Category
{
    public record CategoryDetailsDto(
        CategoryResponseDto Category,
        IReadOnlyList<BookResponseDto> Books
        );

}
