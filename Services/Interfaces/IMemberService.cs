using Library_API.Models;

namespace Library_API.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<Member>> GetAll();
        Task<Member> GetById(int id);
        Task<Member> Create(Member member);
        Task Update(int id, Member member);
        Task Delete(int id);
    }
}
