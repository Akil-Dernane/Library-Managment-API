using Library_API.DTOs.Author;
using Library_API.DTOs.Book;
using Library_API.Models;
using System.Linq.Expressions;

namespace Library_API.Mappings
{
    public static class AuthorMapping
    {
        public static Expression<Func<Book, AuthorResponseDto>>
           ToDtoExpression =>
           a => new AuthorResponseDto(
               a.Id,
               a.Name
               );

        public static AuthorResponseDto ToDto(this Author a)
        {
            return new(
                a.Id,
                a.Name
                );
        }
    }
}
