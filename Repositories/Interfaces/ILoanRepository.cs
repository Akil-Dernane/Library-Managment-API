using Library_API.Models;

namespace Library_API.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        Task<IEnumerable<Loan>> GetAll();
        Task<Loan?> GetById(int id);
        Task<Loan> Create(Loan loan);
        Task<bool> Update(int id, Loan updatedLoan);
        Task<bool> Delete(int id);
    }
}
