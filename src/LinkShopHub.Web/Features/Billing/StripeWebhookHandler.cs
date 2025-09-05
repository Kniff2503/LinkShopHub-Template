using LinkShopHub.Infrastructure.Data;
using LinkShopHub.Web.Services;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace LinkShopHub.Web.Features.Billing;

public class StripeWebhookHandler(AppDbContext db, ILogger<StripeWebhookHandler> log, IEmailService email)
{
    public async Task HandleAsync(string json, string signature, CancellationToken ct = default)
    {
        DotNetEnv.Env.Load();
        var secret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
        var stripeEvent = EventUtility.ConstructEvent(json, signature, secret);

        switch (stripeEvent.Type)
        {
            case "invoice.payment_succeeded":
                await HandlePaymentSucceededAsync((Invoice)stripeEvent.Data.Object, ct);
                break;

            case "invoice.payment_failed":
                await HandlePaymentFailedAsync((Invoice)stripeEvent.Data.Object, ct);
                break;
        }
    }

    private async Task HandlePaymentSucceededAsync(Invoice invoice, CancellationToken ct)
    {
        var subscriptionId = invoice.Metadata?["subscription_id"];
        if (string.IsNullOrEmpty(subscriptionId)) return;

        var sub = await db.Subscriptions
                          .Include(s => s.User)          // 👈 join User
                          .FirstOrDefaultAsync(s => s.StripeSubscriptionId == subscriptionId, ct);
        if (sub is null) return;

        sub.Status = "active";
        await db.SaveChangesAsync(ct);

        log.LogInformation("Payment succeeded for subscription {id}", sub.Id);

        // email to the owner
        await email.SendReceiptAsync(sub.User.Email, invoice.AmountPaid / 100m);
    }

    private async Task HandlePaymentFailedAsync(Invoice invoice, CancellationToken ct)
    {
        var subscriptionId = invoice.Metadata?["subscription_id"];

        if (string.IsNullOrEmpty(subscriptionId)) return;

        var sub = await db.Subscriptions
                          .FirstOrDefaultAsync(s => s.StripeSubscriptionId == subscriptionId, ct);

        if (sub is null) return;

        sub.Status = "past_due";
        log.LogWarning("Payment failed for subscription {id}", sub.Id);
    }
}
