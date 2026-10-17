using Library_API.DTOs.Author;
using Library_API.DTOs.Book;
using Library_API.DTOs.Category;
using Library_API.Models;
using System.Linq.Expressions;

namespace Library_API.Mappings
{
    public static class CategoryMapping
    {
        public static Expression<Func<Category, CategoryResponseDto>>
          ToDtoExpression =>
          c => new CategoryResponseDto(
              c.Id,
              c.Name
              );

        public static Expression<Func<Category, CategoryDetailsDto>>
        ToDetailsDtoExpression =>
        c => new CategoryDetailsDto(
            c.ToDto(),
            c.Books.Select(b => new BookResponseDto(
                b.Id,
                b.Name,
                b.Pages,
                b.IsAvailable
            )).ToList()
        );
        public static CategoryResponseDto ToDto(this Category c)
        {
            return new(
                c.Id,
                c.Name
                );
        }

        public static Category ToEntity(this CreateCategoryDto dto)
        {
            return new()
            {
                Name = dto.Name,
            };
        }

        public static CategoryDetailsDto ToDetailsDto(this Category c)
        {
            return new CategoryDetailsDto(
                c.ToDto(),
                c.Books.Select(b => b.ToDto()).ToList()
                );
        }

        public static void UpdateEntity(this Category category, UpdateCategoryDto dto)
        {
            category.Name = dto.Name;
        }
    }
}
