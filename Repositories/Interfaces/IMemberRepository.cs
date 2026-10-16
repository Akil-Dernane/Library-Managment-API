using Library_API.Models;

namespace Library_API.Repositories.Interfaces
{
    public interface IMemberRepository
    {
        Task<IEnumerable<Member>> GetAll();
        Task<Member?> GetById(int id);
        Task<Member> Create(Member member);
        Task<bool> Update(int id, Member updatedMember);
        Task<bool> Delete(int id);

    }
}
