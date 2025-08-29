namespace LinkShopHub.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;          // public profile url
    public string? StripeCustomerId { get; set; }
    public PlanType CurrentPlan { get; set; } = PlanType.Free;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<Link> Links { get; set; } = new List<Link>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public enum PlanType
{
    Free,
    Small,
    Large
}
