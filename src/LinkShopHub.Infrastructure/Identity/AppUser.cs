using Microsoft.AspNetCore.Identity;

namespace LinkShopHub.Infrastructure.Identity;

public class AppUser : IdentityUser<Guid>
{
    public string Slug { get; set; } = string.Empty;
}
