namespace LinkShopHub.Domain.Entities;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } = default!;
    public Guid ProductId { get; set; } = default!;

    public string StripePaymentIntentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = default!;
    public Product Product { get; set; } = default!;
}

public enum OrderStatus
{
    Pending,
    Paid,
    Failed,
    Refunded
}
