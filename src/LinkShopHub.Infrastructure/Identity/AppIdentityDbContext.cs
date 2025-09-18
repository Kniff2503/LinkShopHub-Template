using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LinkShopHub.Infrastructure.Identity;

public class AppIdentityDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);  // Identity-Defaults (Tabellen mit Guid-Keys)

        // Custom: Slug unique für AppUser
        builder.Entity<AppUser>(entity =>
        {
            entity.HasIndex(e => e.Slug).IsUnique();
        });
    }
}