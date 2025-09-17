using LinkShopHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkShopHub.Web.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuth(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            [FromBody] LoginRequest request,
            SignInManager<AppUser> signInManager,
            HttpContext httpContext) =>
        {
            var result = await signInManager.PasswordSignInAsync(
                request.Email,
                request.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var cookie = httpContext.Response.Headers.SetCookie;
                return Results.Ok(new { cookie });
            }

            return Results.Unauthorized();
        });
    }

    private record LoginRequest(string Email, string Password);
}
