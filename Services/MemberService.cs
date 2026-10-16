using Library_API.Exceptions;
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

        public async Task<Member> Create(Member member)
        {
            return await _memberRepository.Create(member);
        }

        public async Task Delete(int id)
        {
            bool deleted = await _memberRepository.Delete(id);
            if (!deleted)
                throw new MemberNotFoundException(id);
        }

        public async Task<IEnumerable<Member>> GetAll()
        {
            return await _memberRepository.GetAll();
        }

        public async Task<Member> GetById(int id)
        {
            Member? member = await _memberRepository.GetById(id);

            if(member == null)
                throw new MemberNotFoundException(id);

            return member;
        }

        public async Task Update(int id, Member member)
        {
            bool updated = await _memberRepository.Update(id, member);
            if(!updated)
                throw new MemberNotFoundException(id);
        }
    }
}
