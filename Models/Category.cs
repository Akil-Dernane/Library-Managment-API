using System.ComponentModel.DataAnnotations;

namespace Library_API.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required,StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public List<Book> Books { get; set; } = new();
    }
}
