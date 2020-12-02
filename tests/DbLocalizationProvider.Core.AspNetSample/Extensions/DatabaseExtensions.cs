using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DbLocalizationProvider.Core.AspNetSample.Data;
using Microsoft.AspNetCore.Builder;
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
    }
}
