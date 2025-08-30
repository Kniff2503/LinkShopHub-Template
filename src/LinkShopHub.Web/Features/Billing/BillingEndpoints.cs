namespace LinkShopHub.Web.Features.Billing;

public static class BillingEndpoints
{
    public static void MapBilling(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/checkout/create-session",
            async (CreateCheckoutRequest req,
                   StripeCheckoutService svc,
                   CancellationToken ct) =>
            {
                var sessionUrl = await svc.CreateSessionAsync(
                    req.Plan,
                    req.SuccessUrl,
                    req.CancelUrl,
                    ct);

                return Results.Ok(new { url = sessionUrl });
            });

        app.MapPost("/api/webhooks/stripe", async (HttpContext ctx, StripeWebhookHandler handler) =>
        {
            var json = await new StreamReader(ctx.Request.Body).ReadToEndAsync();
            await handler.HandleAsync(json, ctx.Request.Headers["Stripe-Signature"], CancellationToken.None);
            return Results.Ok();
        });
    }
}
