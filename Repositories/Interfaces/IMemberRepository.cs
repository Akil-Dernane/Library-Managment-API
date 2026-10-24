using Library_API.DTOs.Members;
using Library_API.Models;

namespace Library_API.Repositories.Interfaces
{
    public interface IMemberRepository
    {
        Task<IEnumerable<MemberResponseDto>> GetAll();
        Task<Member?> GetById(int id);
        Task<int?> GetMemberIdByUserIdAsync(int userId);
        Task<MemberResponseDto?> GetDtoById(int id);
        Task<Member?> GetByIdWithRelationships(int id);
        Task<Member> Create(Member member);
        Task<bool> Delete(int id);
        Task SaveChangesAsync();

    }
}
