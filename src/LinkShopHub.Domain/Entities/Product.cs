namespace LinkShopHub.Domain.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } = default!;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? FileUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? StripePriceId { get; set; }

    public User User { get; set; } = default!;
}
