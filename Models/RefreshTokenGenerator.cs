using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace Library_API.Models
{
    public static class RefreshTokenGenerator
    {
        public static string Generate()
        {
            return Base64Url.EncodeToString(RandomNumberGenerator.GetBytes(64));
        }

        public static string Hash(string token)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        }
    }
}
