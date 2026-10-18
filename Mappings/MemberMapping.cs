using Library_API.DTOs.Members;
using Library_API.Models;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Library_API.Mappings
{
    public static class MemberMapping
    {
        public static Expression<Func<Member, MemberResponseDto>>
            ToDtoExpression => member => new MemberResponseDto(
                member.Id,
                member.Name,
                member.DateOfBirth,
                member.Loans.Count(l => l.ReturnedAt == null)
                );
        public static MemberResponseDto ToDto(this Member member)
        {
            return new MemberResponseDto(
                member.Id,
                member.Name,
                member.DateOfBirth,
                member.Loans.Count(l => l.ReturnedAt == null)
            );
        }

        public static Member ToEntity(this Member member, CreateMemberDto dto)
        {
            return new()
            {
                Name = dto.Name,
                DateOfBirth = dto.DateOfBirth!.Value
            };
        }

        public static void UpdateEntity(this Member member, UpdateMemberDto dto)
        {
            member.Name = dto.Name;
            member.DateOfBirth = dto.DateOfBirth!.Value;
        }
    }
}
