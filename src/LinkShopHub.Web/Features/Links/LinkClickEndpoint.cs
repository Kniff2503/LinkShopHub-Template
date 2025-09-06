using LinkShopHub.Domain.Entities;
using LinkShopHub.Infrastructure.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace LinkShopHub.Web.Features.Links;

public static class LinkClickEndpoint
{
    public static void MapLinkClicks(this IEndpointRouteBuilder app)
    {
        app.MapGet("/click/{slug}/{linkId:guid}", async (
            string slug,
            Guid linkId,
            HttpContext ctx,
            AppDbContext db,
            NavigationManager nav) =>
        {
            var link = await db.Links
                               .Include(l => l.User)
                               .FirstOrDefaultAsync(l => l.Id == linkId);

            if (link is null) return Results.NotFound();

            // A/B-Variante setzen (falls noch nicht)
            if (string.IsNullOrEmpty(link.AbVariant))
                link.AbVariant = Random.Shared.Next(2) == 0 ? "A" : "B";

            // Klick speichern
            db.ClickEvents.Add(new ClickEvent
            {
                LinkId = link.Id,
                Variant = link.AbVariant,
                Timestamp = DateTime.UtcNow,
                IpAddress = ctx.Connection.RemoteIpAddress?.ToString(),
                Referer = ctx.Request.Headers.Referer.ToString()
            });
            await db.SaveChangesAsync();

            // Weiterleiten
            return Results.Redirect(link.Url);
        });
    }
}
