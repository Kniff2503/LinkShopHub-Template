using LinkShopHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LinkShopHub.Web.Features.Api;

public static class ApiEndpoints
{
    public static void MapApi(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api/v1")
                     .RequireRateLimiting("global");   // ← policy name added

        api.MapGet("/users", async (AppDbContext db) =>
            await db.Users.Select(u => new { u.Id, u.Email, u.CurrentPlan }).ToListAsync());

        api.MapGet("/links/{slug}", async (string slug, AppDbContext db) =>
            await db.Links
                    .Where(l => l.User.Slug == slug)
                    .Select(l => new { l.Id, l.Label, l.Url, l.ThumbnailUrl })
                    .ToListAsync());

        api.MapPost("/webhooks/public", async (HttpContext ctx, AppDbContext db) =>
        {
            var json = await new StreamReader(ctx.Request.Body).ReadToEndAsync();
            // TODO: validate, store, forward
            return Results.Ok();
        });
    }
}
