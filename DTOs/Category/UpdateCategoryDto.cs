using System.ComponentModel.DataAnnotations;

namespace Library_API.DTOs.Category
{
    public class UpdateCategoryDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;
        public List<int>? BookIds { get; set; }
    }
}
