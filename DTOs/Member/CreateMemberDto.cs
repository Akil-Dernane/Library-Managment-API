
using System.ComponentModel.DataAnnotations;

namespace Library_API.DTOs.Members;

public class CreateMemberDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateOnly? DateOfBirth { get; set; }
}
