using Microsoft.AspNetCore.Identity;

namespace Library_API.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
