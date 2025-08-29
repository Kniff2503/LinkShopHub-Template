namespace LinkShopHub.Domain.Entities;

public class Link
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } = default!;
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int SortOrder { get; set; }
    public LinkType Type { get; set; } = LinkType.Link;

    public User User { get; set; } = default!;
}

public enum LinkType
{
    Link,
    Product,
    Affiliate
}
