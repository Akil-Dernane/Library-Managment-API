using System.ComponentModel.DataAnnotations;

namespace Library_API.DTOs.Book
{
    public class CreateBookDto
    {
        [Required, MinLength(1)]
        public string Name { get; set; } = string.Empty;
        [Required,Range(1,int.MaxValue)]
        public int Pages { get; set; }
        public List<int> AuthorIds { get; set; } = new();
        public List<int> CategoryIds { get; set; } = new();
    }
}
