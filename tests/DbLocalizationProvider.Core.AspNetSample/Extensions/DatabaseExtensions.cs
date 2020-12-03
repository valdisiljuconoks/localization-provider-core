using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

            //var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //roleManager.CreateAsync(new IdentityRole("Administrators")).Wait();
            
            //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            //var user = userManager.FindByEmailAsync("semack@gmail.com").Result;
            //var res = userManager.AddToRoleAsync(user, "Administrators").Result;

        }
    }
}
