
namespace Library_API.DTOs.Members;

public record MemberResponseDto(
    int Id,
    string Name,
    DateOnly DateOfBirth,
    int ActiveLoansCount
);
