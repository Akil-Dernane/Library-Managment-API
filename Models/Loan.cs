using Library_API.Exceptions.Loan;
using System.ComponentModel.DataAnnotations;

namespace Library_API.Models
{
    public class Loan
    {
        public int Id { get; set; }
        [Required, Range(1,int.MaxValue)]
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;
        [Required, Range(1, int.MaxValue)]
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public DateTime BorrowedAt { get; set; }
        public DateTime? ReturnedAt { get; set; }

        public void Return()
        {
            if (ReturnedAt != null)
                throw new LoanAlreadyReturnedException(Id);

            ReturnedAt = DateTime.UtcNow;
        }
    }
}
