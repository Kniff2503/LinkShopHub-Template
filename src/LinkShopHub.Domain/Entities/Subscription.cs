namespace LinkShopHub.Domain.Entities;

public class Subscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } = default!;
    public string? StripeSubscriptionId { get; set; }
    public PlanType Plan { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }

    public User User { get; set; } = default!;
}
