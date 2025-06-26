using ClassroomManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClassroomManagement.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await context.Database.MigrateAsync();

            string[] roles = new[] { "Admin", "Instructor", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL");
            var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

            Console.WriteLine($"Admin email: {adminEmail}");
            Console.WriteLine($"Admin password: {(string.IsNullOrEmpty(adminPassword) ? "NOT SET" : "[HIDDEN]")}");

            if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
            {
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "System",
                        LastName = "Admin",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(admin, adminPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                        Console.WriteLine("Admin user created successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to create admin user:");
                        foreach (var error in result.Errors)
                            Console.WriteLine($"- {error.Description}");
                    }
                }
                else
                {
                    Console.WriteLine("Admin user already exists.");
                }
            }
            else
            {
                Console.WriteLine("ADMIN_EMAIL or ADMIN_PASSWORD not set in environment.");
            }
        }
    }
}
