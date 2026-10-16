using Library_API.Data;
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

        public async Task<IEnumerable<Member>> GetAll()
        {
            return await _context.Members.AsNoTracking().ToListAsync();
        }

        public async Task<Member?> GetById(int id)
        {
            return await _context.Members.FindAsync(id);
        }

        public async Task<bool> Update(int id, Member updatedMember)
        {
            Member? member = await _context.Members.FindAsync(id);
            if (member == null)
                return false;

            member.Name = updatedMember.Name;
            member.DateOfBirth = updatedMember.DateOfBirth;

            return true;
        }
    }
}
