using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DbLocalizationProvider.Core.AspNetSample.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DbLocalizationProvider.Core.AspNetSample.Extensions
{
    public static class DatabaseExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
        }

        public static void SeedUserData(this IApplicationBuilder app, string email, string password, string role)
        {
            Task.Run(async () =>
                {
                    using var scope = app.ApplicationServices.CreateScope();
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                    if (await userManager.FindByEmailAsync(email) == null)
                    {
                        var user = new IdentityUser(email);
                        await userManager.CreateAsync(user, password);
                        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            ).Wait();
        }
    }
}
