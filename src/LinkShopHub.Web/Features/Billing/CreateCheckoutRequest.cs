using LinkShopHub.Domain.Entities;

namespace LinkShopHub.Web.Features.Billing;

public record CreateCheckoutRequest(
    PlanType Plan,
    string SuccessUrl,
    string CancelUrl);
