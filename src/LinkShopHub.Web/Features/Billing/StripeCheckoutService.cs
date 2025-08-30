using LinkShopHub.Domain.Entities;
using Stripe;
using Stripe.Checkout;

namespace LinkShopHub.Web.Features.Billing;

public class StripeCheckoutService()
{
    public async Task<string> CreateSessionAsync(
        PlanType plan,
        string successUrl,
        string cancelUrl,
        CancellationToken ct = default)
    {
        DotNetEnv.Env.Load();
        var stripeKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");


        if (string.IsNullOrEmpty(stripeKey))
        {
            throw new Exception("Stripe API Key is missing.");
        }

        StripeConfiguration.ApiKey = stripeKey;

        var priceId = plan switch
        {
            PlanType.Basic => "price_1S1Yv4DGawOzlPBo1qH7lIbu",
            PlanType.Pro => "price_1S1YvfDGawOzlPBo9AdhwTYV",
            _ => throw new ArgumentOutOfRangeException(nameof(plan))
        };

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            Mode = "subscription",
            LineItems = new List<SessionLineItemOptions>
            {
                new() { Price = priceId, Quantity = 1 }
            },
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options, cancellationToken: ct);
        return session.Url;
    }
}
