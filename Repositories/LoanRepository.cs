using Library_API.Data;
using Library_API.Models;
using Library_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly AppDbContext _context;
        public LoanRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Loan> Create(Loan loan)
        {
            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task<bool> Delete(int id)
        {
            Loan? loan = await _context.Loans.FindAsync(id);
            if (loan == null)
                return false;

            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Loan>> GetAll()
        {
            return await _context.Loans.AsNoTracking().ToListAsync();
        }

        public async Task<Loan?> GetById(int id)
        {
            return await _context.Loans.FindAsync(id);
        }

        public async Task<bool> Update(int id, Loan updatedLoan)
        {
            Loan? loan = await _context.Loans.FindAsync(id);
            if (loan == null)
                return false;

            loan.Bookid = updatedLoan.Bookid;
            loan.Memberid = updatedLoan.Memberid;
            loan.BorrowedAt = updatedLoan.BorrowedAt;
            loan.ReturnedAt = updatedLoan.ReturnedAt;

            return true;
        }
    }
}
