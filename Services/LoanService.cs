using Library_API.Models;
using Library_API.Exceptions;
using Library_API.Repositories.Interfaces;
using Library_API.Services.Interfaces;

namespace Library_API.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;

        public LoanService(ILoanRepository loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<Loan> Create(Loan loan)
        {
            return await _loanRepository.Create(loan);
        }

        public async Task Delete(int id)
        {
            bool deleted = await _loanRepository.Delete(id);
            if (!deleted)
                throw new LoanNotFoundException(id);
        }

        public async Task<IEnumerable<Loan>> GetAll()
        {
            return await _loanRepository.GetAll();
        }

        public async Task<Loan> GetById(int id)
        {
            Loan? loan = await _loanRepository.GetById(id);

            if(loan == null)
                throw new LoanNotFoundException(id);

            return loan;
        }

        public async Task Update(int id, Loan loan)
        {
            bool updated = await _loanRepository.Update(id, loan);
            if(!updated)
                throw new LoanNotFoundException(id);
        }
    }
}
