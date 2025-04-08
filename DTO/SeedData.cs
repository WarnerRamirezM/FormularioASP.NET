using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;

namespace WebApplication1.DTO
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var roleNames = new[] { "Admin", "Usuario", "Moderador", "Invitado" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var user = await userManager.FindByEmailAsync("admin@admin.com");
            if (user == null)
            {
                user = new ApplicationUser()
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    nombre = "Administrador"
                };
                await userManager.CreateAsync(user, "Password123!");

                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
