using Library_API.DTOs.Members;
using Library_API.Exceptions.Member;
using Library_API.Mappings;
using Library_API.Models;
using Library_API.Repositories.Interfaces;
using Library_API.Services.Interfaces;

namespace Library_API.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<MemberResponseDto> Create(CreateMemberDto dto)
        {
            Member member = new Member().ToEntity(dto);
            return (await _memberRepository.Create(member)).ToDto();
        }

        public async Task Delete(int id)
        {
            bool deleted = await _memberRepository.Delete(id);
            if (!deleted)
                throw new MemberNotFoundException(id);
        }

        public async Task<IEnumerable<MemberResponseDto>> GetAll()
        {
            return await _memberRepository.GetAll();
        }

        public async Task<MemberResponseDto> GetDtoById(int id)
        {
            return await _memberRepository.GetDtoById(id) ?? throw new MemberNotFoundException(id);
        }

        public async Task<Member?> GetById(int id)
        {
            Member member = await _memberRepository.GetById(id) ?? throw new MemberNotFoundException(id);

            return member;
        }

        public async Task Update(int id, UpdateMemberDto dto)
        {
            Member member = await _memberRepository.GetById(id) ?? throw new MemberNotFoundException(id);
            member.UpdateEntity(dto);
            await _memberRepository.SaveChangesAsync();
        }
    }
}
