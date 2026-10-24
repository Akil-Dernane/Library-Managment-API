using System.ComponentModel.DataAnnotations;

namespace Library_API.DTOs.ApplicationUser
{
    public class AssignRoleDto
    {
        [Required, StringLength(50)]
        public string Role { get; set; } = string.Empty;
    }
}
