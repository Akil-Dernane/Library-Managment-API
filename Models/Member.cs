using System.ComponentModel.DataAnnotations;

namespace Library_API.Models
{
    public class Member
    {
        public int Id { get; set; }
        [Required,StringLength(100)]
        public string? Name { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }

        public List<Loan>? Loans { get; set; } = new();
    }
}
