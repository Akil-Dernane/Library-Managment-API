using Library_API.Models;

namespace Library_API.Services.Interfaces
{
    public interface ILoanService
    {
        Task<IEnumerable<Loan>> GetAll();
        Task<Loan> GetById(int id);
        Task<Loan> Create(Loan loan);
        Task Update(int id, Loan loan);
        Task Delete(int id);
    }
}
