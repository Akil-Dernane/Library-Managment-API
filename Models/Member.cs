
using Library_API.Exceptions.Member;
using System.ComponentModel.DataAnnotations;

namespace Library_API.Models;

public class Member
{
    public int Id { get; set; }

    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateOnly DateOfBirth { get; set; }

    public List<Loan> Loans { get; set; } = new();

    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public void EnsureCanBorrow(int borrowingLimit)
    {
        if (borrowingLimit < 1)
            throw new ArgumentOutOfRangeException(nameof(borrowingLimit));

        int activeLoans = Loans.Count(l => l.ReturnedAt == null);

        if (activeLoans >= borrowingLimit)
            throw new BorrowingLimitExceededException(Id);
    }
}
