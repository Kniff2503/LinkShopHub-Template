using LinkShopHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkShopHub.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Link> Links => Set<Link>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<ClickEvent> ClickEvents => Set<ClickEvent>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
               .HasIndex(u => u.Email)
               .IsUnique();

        builder.Entity<User>()
               .HasIndex(u => u.Slug)
               .IsUnique();

        builder.Entity<Link>()
               .HasOne(l => l.User)
               .WithMany(u => u.Links)
               .HasForeignKey(l => l.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Order>()
               .HasOne(o => o.User)
               .WithMany(u => u.Orders)
               .HasForeignKey(o => o.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
