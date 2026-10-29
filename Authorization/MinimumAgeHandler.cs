using Library_API.Data;
using Library_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Authorization
{
    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;
        public MinimumAgeHandler(AppDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            if (!int.TryParse(_currentUser.MemberId, out int member_id))
                return;

            DateOnly? dateOfBirth = await _context.Members
                .Where(m => m.Id == member_id)
                .Select(m => m.DateOfBirth)
                .FirstOrDefaultAsync();
            if (dateOfBirth == null)
                return;

            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

            int age = today.Year - dateOfBirth.Value.Year;

            if (dateOfBirth.Value > today.AddYears(-age))
                age--;

            if (age >= requirement.MinimumAge)
                context.Succeed(requirement);
        }
    }
}
