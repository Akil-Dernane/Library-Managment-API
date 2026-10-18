
using System.ComponentModel.DataAnnotations;
using Library_API.Models;

namespace Library_API.DTOs.Members;

public class UpdateMemberDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateOnly? DateOfBirth { get; set; }
}
