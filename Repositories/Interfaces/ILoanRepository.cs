using Library_API.DTOs.Loan;
using Library_API.Models;

namespace Library_API.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        Task<IEnumerable<LoanResponseDto>> GetAll();
        Task<Loan?> GetById(int id);
        Task<IEnumerable<LoanResponseDto>> GetMyLoans(int memberId);
        Task<Loan?> GetByIdWithRelationships(int id);
        Task<Loan> Create(Loan loan);
        Task SaveChangesAsync();
        Task<bool> Delete(int id);
    }
}
