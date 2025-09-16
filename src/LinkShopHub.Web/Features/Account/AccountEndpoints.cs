using LinkShopHub.Infrastructure.Identity;
using LinkShopHub.Web.Services;
using Microsoft.AspNetCore.Identity;

namespace LinkShopHub.Web.Features.Account;

public static class AccountEndpoints
{
    public static void MapAccount(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/account/change-email", async (
                ChangeEmailRequest request,
                UserManager<AppUser> userManager,
                IEmailService emailService,
                HttpContext httpContext) =>
        {
            var user = await userManager.GetUserAsync(httpContext.User);
            if (user == null) return Results.Unauthorized();

            var token = await userManager.GenerateChangeEmailTokenAsync(user, request.NewEmail);
            var callbackUrl = httpContext.Request.Scheme + "://" + httpContext.Request.Host +
                              $"/account/confirm-email-change?userId={user.Id}&newEmail={Uri.EscapeDataString(request.NewEmail)}&token={Uri.EscapeDataString(token)}";

            await emailService.SendEmailChangeAsync(user.Email, callbackUrl);
            return Results.Ok(new { message = "Check your inbox for the confirmation link." });
        });

        endpoints.MapPost("/api/account/change-password", async (
                ChangePasswordRequest request,
                UserManager<AppUser> userManager,
                HttpContext httpContext) =>
        {
            var user = await userManager.GetUserAsync(httpContext.User);
            if (user == null) return Results.Unauthorized();

            var result = await userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            if (result.Succeeded)
                return Results.Ok(new { message = "Password changed successfully." });

            return Results.BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        });

        // Placeholder for future endpoints (#28b, #28c, #28d)
        // endpoints.MapPost("/api/account/change-password", …);
        // endpoints.MapDelete("/api/account/delete", …);
        // endpoints.MapPost("/api/account/avatar", …);
    }
}

public record ChangeEmailRequest(string NewEmail);
public record ChangePasswordRequest(string OldPassword, string NewPassword);
