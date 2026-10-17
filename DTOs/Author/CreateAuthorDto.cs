using System.ComponentModel.DataAnnotations;

namespace Library_API.DTOs.Author
{
    public class CreateAuthorDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public List<int> BookIds { get; set; } = new();
    }
}
