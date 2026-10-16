namespace Library_API.DTOs.Book
{
    public record BookResponseDto(
        int Id,
        string Name,
        int pages,
        bool IsAvailable
        );
}
