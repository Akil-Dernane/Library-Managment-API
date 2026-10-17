using Library_API.DTOs.Book;

namespace Library_API.DTOs.Author
{
    public record AuthorDetailsDto
    (
        AuthorResponseDto Author,
        IReadOnlyList<BookResponseDto> Books
    );
}
