namespace Library_API.DTOs.Loan
{
    public record LoanResponseDto(
     int Id,
     int BookId,
     string BookTitle,
     int MemberId,
     string MemberName,
     DateTime BorrowedAt,
     DateTime? ReturnedAt
 );
}
