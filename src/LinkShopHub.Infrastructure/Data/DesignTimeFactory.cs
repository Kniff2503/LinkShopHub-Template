using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LinkShopHub.Infrastructure.Data;

public class DesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json")
                     .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                             .UseNpgsql(config.GetConnectionString("Default"));

        return new AppDbContext(optionsBuilder.Options);
    }
}
