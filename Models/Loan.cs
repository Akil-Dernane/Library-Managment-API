using System.ComponentModel.DataAnnotations;

namespace Library_API.Models
{
    public class Loan
    {
        public int Id { get; set; }
        [Required]
        public int? Bookid { get; set; }
        public Book Book { get; set; }
        [Required]
        public int? Memberid { get; set; }
        public Member Member { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }
}
