using Library_API.Data;
using Library_API.DTOs.Members;
using Library_API.Mappings;
using Library_API.Models;
using Library_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly AppDbContext _context;
        public MemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<Member> Create(Member member)
        {
            await _context.Members.AddAsync(member);
            await _context.SaveChangesAsync();
            return member;
        }

        public async Task<bool> Delete(int id)
        {
            Member? member = await _context.Members.FindAsync(id);
            if(member == null)
                return false;

            _context.Members.Remove(member);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<MemberResponseDto>> GetAll()
        {
            return await _context.Members
                .Select(MemberMapping.ToDtoExpression)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Member?> GetById(int id)
        {
            return await _context.Members.FindAsync(id);
        }
        public async Task<MemberResponseDto?> GetDtoById(int id)
        {
            return await _context.Members
               .Where(m => m.Id == id)
               .Select(MemberMapping.ToDtoExpression)
               .FirstOrDefaultAsync();
        }

        public async Task<Member?> GetByIdWithRelationships(int id)
        {
            return await _context.Members
                .Include(m => m.Loans)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

    }
}
