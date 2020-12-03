using System.IO;
using DbLocalizationProvider.AspNetCore.EntityFramework.Extensions;
using DbLocalizationProvider.Core.AspNetSample.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DbLocalizationProvider.Core.AspNetSample.Factories
{
    public class DesignTimeApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseNpgsql(config.GetConnectionString("PostgreSqlConnection"));
            optionsBuilder.UseLocalizationProvider();

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
