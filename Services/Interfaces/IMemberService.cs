using Library_API.DTOs.Members;
using Library_API.Models;

namespace Library_API.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberResponseDto>> GetAll();
        Task<MemberResponseDto> GetDtoById(int id);
        Task<Member?> GetById(int id);
        Task<MemberResponseDto> Create(CreateMemberDto dto);
        Task Update(int id, UpdateMemberDto dto);
        Task Delete(int id);
    }
}
