using Library_API.DTOs.Author;
using Library_API.DTOs.Book;
using Library_API.Models;
using System.Linq.Expressions;

namespace Library_API.Mappings
{
    public static class AuthorMapping
    {
        public static Expression<Func<Author, AuthorResponseDto>>
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

        public static AuthorDetailsDto ToDetailsDto(this Author a)
        {
            return new(
                a.ToDto(),
                a.Books.Select(a => a.ToDto()).ToList()
                );
        }

        public static Author ToEntity(this CreateAuthorDto dto)
        {
            return new()
            {
                Name = dto.Name,
            };
        }
    }
}
