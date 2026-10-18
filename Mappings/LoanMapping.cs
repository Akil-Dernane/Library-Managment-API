using Library_API.DTOs.Loan;
using Library_API.Models;
using System.Linq.Expressions;

namespace Library_API.Mappings
{
    public static class LoanMapping
    {
        public static Expression<Func<Loan, LoanResponseDto>>
            ToDtoExpression => loan => new LoanResponseDto(
                loan.Id,
                loan.BookId,
                loan.Book.Name,
                loan.MemberId,
                loan.Member.Name,
                loan.BorrowedAt,
                loan.ReturnedAt
                );
        public static LoanResponseDto ToDto(this Loan loan)
        {
            return new LoanResponseDto(
                loan.Id,
                loan.BookId,
                loan.Book.Name,
                loan.MemberId,
                loan.Member.Name,
                loan.BorrowedAt,
                loan.ReturnedAt
            );
        }
        public static Loan ToEntity(this CreateLoanDto dto)
        {
            return new()
            {
                BookId = dto.BookId,
                MemberId = dto.MemberId
            };
        }
    }
}
