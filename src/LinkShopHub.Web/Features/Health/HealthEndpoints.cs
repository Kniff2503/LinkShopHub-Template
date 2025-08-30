namespace LinkShopHub.Web.Features.Health;

public static class HealthEndpoints
{
    public static void MapHealth(this IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok("alive"));
    }
}
