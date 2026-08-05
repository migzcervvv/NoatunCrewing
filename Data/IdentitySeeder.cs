namespace NoatunCrewing.Data.Seed;

// Replaces the MVC5 Configuration.cs seed method / IdentitySeeder run on app start.
// Call IdentitySeeder.SeedAsync(app.Services) once from Program.cs after the app is built.
public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        foreach (var roleName in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Bootstrap SuperAdmin only if no user holds that role yet. Credentials come from
        // configuration/user-secrets, never hardcoded, and this is a one-time bootstrap path.
        var superAdmins = await userManager.GetUsersInRoleAsync(AppRoles.SuperAdmin);
        if (superAdmins.Count == 0)
        {
            var bootstrapEmail = "it@aviormarine.com";
            var bootstrapPassword = "Avior@2026!";

            if (!string.IsNullOrWhiteSpace(bootstrapEmail) && !string.IsNullOrWhiteSpace(bootstrapPassword))
            {
                var user = new ApplicationUser
                {
                    UserName = bootstrapEmail,
                    Email = bootstrapEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, bootstrapPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, AppRoles.SuperAdmin);
                }
            }
        }
    }
}
