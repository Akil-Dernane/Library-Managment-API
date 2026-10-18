using Library_API.DTOs.Book;
using Library_API.DTOs.Loan;
using Library_API.Models;

namespace Library_API.Services.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanResponseDto>> GetAll();
        Task<LoanResponseDto> GetById(int id);
        Task<LoanResponseDto> Create(CreateLoanDto dto);
        Task ReturnBook(int loanId);
        Task Delete(int id);
    }
}
