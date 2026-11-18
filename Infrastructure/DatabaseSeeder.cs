using HouseholdManager.Database;
using HouseholdManager.Database.DataModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HouseholdManager.Infrastructure
{
    public static class DatabaseSeeder
    {
        private static readonly string[] RoleNames = ["admin", "child"];

        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<HouseholdContext>();
            await context.Database.MigrateAsync();

            await EnsureHouseholdAsync(context);
            await EnsureRolesAsync(scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>());
            await EnsureAdminAsync(context, scope.ServiceProvider.GetRequiredService<UserManager<User>>(), configuration);
        }

        private static async Task EnsureHouseholdAsync(HouseholdContext context)
        {
            if (await context.Households.AnyAsync()) return;

            context.Households.Add(new Household { Name = "Test household" });
            await context.SaveChangesAsync();
        }

        private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in RoleNames)
            {
                if (await roleManager.RoleExistsAsync(roleName)) continue;
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private static async Task EnsureAdminAsync(HouseholdContext context, UserManager<User> userManager, IConfiguration configuration)
        {
            var adminUserName = configuration["SeedAdmin:UserName"];
            var adminPassword = configuration["SeedAdmin:Password"];
            if (adminUserName is null || adminPassword is null) return;
            if (await userManager.FindByNameAsync(adminUserName) is not null) return;

            var household = await context.Households.OrderBy(household => household.Id).FirstAsync();
            var admin = new User { UserName = adminUserName, HouseholdId = household.Id };

            var createResult = await userManager.CreateAsync(admin, adminPassword);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", createResult.Errors.Select(error => error.Description)));
            }

            var roleResult = await userManager.AddToRoleAsync(admin, "admin");
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join(", ", roleResult.Errors.Select(error => error.Description)));
            }
        }
    }
}
