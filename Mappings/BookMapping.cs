using Library_API.DTOs.Book;
using Library_API.Models;
using System.Linq.Expressions;

namespace Library_API.Mappings
{
    public static class BookMapping
    {
        public static Expression<Func<Book, BookResponseDto>>
            ToDtoExpression => 
            b => new BookResponseDto(
                b.Id,
                b.Name,
                b.Pages,
                b.IsAvailable
                ); 
        public static BookResponseDto ToDto(this Book b)
        {
            return new(
                b.Id,
                b.Name,
                b.Pages,
                b.IsAvailable
                );
        }
        public static BookDetailsDto ToDetailsDto(this Book b)
        {
            return new(
                b.ToDto(),
                b.Authors.Select(a => a.ToDto()).ToList(),
                b.Categories.Select(c => c.ToDto()).ToList()
                );
        }

        public static Book ToEntity(this CreateBookDto dto)
        {
            return new()
            {
                Name = dto.Name,
                Pages = dto.Pages,
            };
        }

        public static void UpdateEntity(this UpdateBookDto dto, Book b)
        {
            b.Name = dto.Name;
            b.Pages = dto.Pages;
            b.IsAvailable = dto.IsAvailable;
        } 
    }
}
