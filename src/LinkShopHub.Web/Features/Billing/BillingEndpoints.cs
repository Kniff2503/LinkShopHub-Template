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
    }
}
