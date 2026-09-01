using RealEstateSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace RealEstateSystem.Data
{
    public static class DbSeeder
    {
        public static void SeedAdmin(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string adminEmail = "admin@realestate.com";

            if (userManager.FindByEmailAsync(adminEmail).Result == null)
            {
                ApplicationUser admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = userManager.CreateAsync(admin, "Admin@123").Result;

                if (result.Succeeded)
                    userManager.AddToRoleAsync(admin, "Admin").Wait();
            }
        }
    }
}
