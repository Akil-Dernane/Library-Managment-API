using System.ComponentModel.DataAnnotations;

namespace Library_API.DTOs.Book
{
    public class UpdateBookDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required,Range(1,int.MaxValue)]
        public int Pages { get; set; }

        [Required]
        public bool IsAvailable { get; set; }
        public List<int>? AuthorIds { get; set; }
        public List<int>? CategoryIds { get; set; }

    }
}
