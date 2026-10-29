using Library_API.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Library_API.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            //Seeding Roles
            foreach (string role in new[] { Roles.Admin, Roles.Librarian, Roles.Member})
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
            }

            //Seeding An Admin
            string? email = config["SeedAdmin:Email"];
            string? password = config["SeedAdmin:Password"];

            if (email is not null && password is not null)
            {
                if (await userManager.FindByEmailAsync(email) is null)
                {
                    var admin = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                    IdentityResult result = await userManager.CreateAsync(admin, password);
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(admin, Roles.Admin);
                }
            }

            //Seeding Roles Permission
            foreach(var (rolename, permissions) in RolePermissionMap.Defaults)
            {
                var role = await roleManager.FindByNameAsync(rolename);
                if (role is null) continue;

                var existing = (await roleManager.GetClaimsAsync(role))
                    .Where(c => c.Type == AppClaims.Permission)
                    .Select(c => c.Value)
                    .ToHashSet();

                foreach(String perrmission in permissions.Where(p => !existing.Contains(p)))
                {
                    await roleManager.AddClaimAsync(role, new Claim(AppClaims.Permission, perrmission));
                }
            }

        }
    }
}
