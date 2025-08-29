namespace LinkShopHub.Domain.Entities;

public class ClickEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LinkId { get; set; } = default!;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Referer { get; set; }
    public string? IpAddress { get; set; }
    public string? Country { get; set; }

    public Link Link { get; set; } = default!;
}
