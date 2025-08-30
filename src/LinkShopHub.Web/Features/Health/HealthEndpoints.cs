namespace LinkShopHub.Web.Features.Health;

public static class HealthEndpoints
{
    public static void MapHealth(this IEndpointRouteBuilder app)
    {
        app.MapMethods("/health", new[] { "GET", "HEAD" }, () => StatusCodes.Status200OK);
    }
}
