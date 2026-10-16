using Library_API.DTOs.Author;
using Library_API.DTOs.Category;
using Library_API.Models;
using System.Linq.Expressions;

namespace Library_API.Mappings
{
    public static class CategoryMapping
    {
        public static Expression<Func<Book, CategoryResponseDto>>
          ToDtoExpression =>
          c => new CategoryResponseDto(
              c.Id,
              c.Name
              );

        public static CategoryResponseDto ToDto(this Category c)
        {
            return new(
                c.Id,
                c.Name
                );
        }
    }
}
