using System.Security.Claims;

namespace Library_API.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateAccessToken(int userId, string email, IEnumerable<Claim> userClaims);
    }
}
