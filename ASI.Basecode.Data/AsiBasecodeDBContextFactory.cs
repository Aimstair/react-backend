using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ASI.Basecode.Data
{
    /// <summary>
    /// Design-time factory for creating DbContext during migrations
    /// </summary>
    public class AsiBasecodeDBContextFactory : IDesignTimeDbContextFactory<AsiBasecodeDBContext>
    {
        public AsiBasecodeDBContext CreateDbContext(string[] args)
        {
            // Build configuration from appsettings.json
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ASI.Basecode.WebApp");
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AsiBasecodeDBContext>();
            optionsBuilder.UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.CommandTimeout(120);
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: System.TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

            return new AsiBasecodeDBContext(optionsBuilder.Options);
        }
    }
}


