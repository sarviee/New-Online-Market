using Microsoft.AspNetCore.Identity;
using OnlineMarket.Models;


public class SeedRoles
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // Seed the roles
            var roles = new[] { "Admin", "Seller", "Client" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                    {
                        Console.WriteLine($"Failed to create role {role}: {string.Join(", ", roleResult.Errors)}");
                    }
                }
            }

            var email = "admin@mail.com";
            var password = "Admin.123";
            var adminUser = await userManager.FindByEmailAsync(email);

            // Create the admin user if not already created
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true  // Mark email as confirmed for simplicity
                };

                var createUserResult = await userManager.CreateAsync(adminUser, password);

                if (!createUserResult.Succeeded)
                {
                    Console.WriteLine($"Failed to create admin user: {string.Join(", ", createUserResult.Errors)}");
                    return;  // Exit if user creation fails
                }

                Console.WriteLine($"Admin user {adminUser.UserName} created successfully.");
            }

            // Check if user already has the Admin role
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                var addRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");

                if (!addRoleResult.Succeeded)
                {
                    Console.WriteLine($"Failed to add Admin role to {adminUser.UserName}: {string.Join(", ", addRoleResult.Errors)}");
                }
                else
                {
                    Console.WriteLine($"Admin role assigned to {adminUser.UserName}");
                }
            }
            else
            {
                Console.WriteLine($"{adminUser.UserName} already has the Admin role.");
            }
        }
    }
}
