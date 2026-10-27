using System.ComponentModel.DataAnnotations;

public class ClaimRequest
{
    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public string Value { get; set; } = string.Empty;
}