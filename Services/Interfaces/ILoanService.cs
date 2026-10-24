using Library_API.DTOs.Book;
using Library_API.DTOs.Loan;
using Library_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library_API.Services.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanResponseDto>> GetAll();
        Task<LoanResponseDto> GetById(int id);

        Task<IEnumerable<LoanResponseDto>> GetMyLoans(int id);
        Task<LoanResponseDto> Create(CreateLoanDto dto);
        Task ReturnBook(int loanId);
        Task Delete(int id);
    }
}
