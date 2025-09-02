using LinkShopHub.Domain.Entities;
using LinkShopHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LinkShopHub.Web.Data;

public static class DemoSeed
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Users.AnyAsync())
        {
            db.Users.Add(new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Email = "demo@local.dev",
                Slug = "demo",
                CurrentPlan = PlanType.Free,
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }
    }
}
