using System.ComponentModel.DataAnnotations;

namespace Library_API.DTOs.Auth
{
    public class RegisterDto
    {
        [Required, EmailAddress, StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateOnly? DateOfBirth { get; set; }
    }
}
