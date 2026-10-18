using Library_API.Data;
using Library_API.DTOs.Loan;
using Library_API.Mappings;
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

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
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

        public async Task<IEnumerable<LoanResponseDto>> GetAll()
        {
            return await _context.Loans.AsNoTracking()
                .Select(LoanMapping.ToDtoExpression)
                .ToListAsync();
        }

        public async Task<Loan?> GetById(int id)
        {
            return await _context.Loans.FindAsync(id);
        }
        public async Task<Loan?> GetByIdWithRelationships(int id)
        {
            return await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .AsSplitQuery().FirstOrDefaultAsync(l => l.Id == id);
        }

    }
}
