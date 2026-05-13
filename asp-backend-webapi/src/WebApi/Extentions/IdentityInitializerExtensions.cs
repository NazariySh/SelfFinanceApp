using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Shared.Enums;

namespace WebApi.Extentions
{
    public static class IdentityInitializerExtensions
    {
        public static async Task InitializeIdentityRolesAsync(this IApplicationBuilder app)
        {
            await using var scope = app.ApplicationServices.CreateAsyncScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in Enum.GetNames(typeof(RoleType)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task InitializeSuperAdminAsync(this IApplicationBuilder app)
        {
            await using var scope = app.ApplicationServices.CreateAsyncScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var adminSettings = configuration.GetSection("SuperAdmin");

            var userName = adminSettings["UserName"] ?? throw new ArgumentException("UserName is not found in SuperAdmin section");

            var admin = await userManager.FindByNameAsync(userName);

            if (admin is null)
            {
                var password = adminSettings["Password"] ?? throw new ArgumentException("Password is not found in SuperAdmin section");

                var user = new AppUser
                {
                    UserName = userName,
                    Email = adminSettings["Email"] ?? throw new ArgumentException("Email is not found in SuperAdmin section"),
                    PasswordHash = password,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, nameof(RoleType.Admin));
                }
            }
        }
    }
}
