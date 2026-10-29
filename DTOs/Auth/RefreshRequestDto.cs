using System.ComponentModel.DataAnnotations;

namespace Library_API.DTOs.Auth
{
    public class RefreshRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
