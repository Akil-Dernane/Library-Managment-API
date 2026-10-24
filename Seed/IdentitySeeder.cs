using Library_API.Models;
using Microsoft.AspNetCore.Identity;

namespace Library_API.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();


            foreach (string role in new[] { Roles.Admin, Roles.Librarian, Roles.Member})
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
            }

            string? email = config["SeedAdmin:Email"];
            string? password = config["SeedAdmin:Password"];

            if (email is null || password is null) return;

            if (await userManager.FindByEmailAsync(email) is null)
            {
                var admin = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, Roles.Admin);
            }
        }
    }
}
