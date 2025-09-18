using LinkShopHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace LinkShopHub.Web.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuth(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            HttpContext httpContext,
            SignInManager<AppUser> signInManager) =>
        {
            // Logging: Content-Type + Body (vor allem)
            var contentType = httpContext.Request.ContentType ?? "null";
            var bodyStream = httpContext.Request.Body;
            // bodyStream.Position = 0;  // Reset für Lesen
            var body = await new StreamReader(bodyStream).ReadToEndAsync();
            Console.WriteLine($"=== LOGIN DEBUG ===");
            Console.WriteLine($"Content-Type: '{contentType}'");
            Console.WriteLine($"Raw Body: '{body}' (Length: {body.Length})");
            Console.WriteLine($"===================");

            // KEIN Content-Type-Check (temporär – testet, ob's der ist)
            // if (!contentType?.StartsWith("application/json") ?? true) { ... }  // Auskommentiert

            // Parse JSON (tolerant, case-insensitive)
            LoginRequest? request = null;
            try
            {
                if (string.IsNullOrWhiteSpace(body))
                {
                    Console.WriteLine("ERROR: Empty body – no JSON to parse");
                    return Results.BadRequest("Empty request body.");
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,  // email -> Email, password -> Password
                    AllowTrailingCommas = true  // Tolerant für JS-JSON
                };

                request = JsonSerializer.Deserialize<LoginRequest>(body, options);
                Console.WriteLine($"Parsed Request: Email='{request?.Email}', Password='{request?.Password ?? "NULL"}'");
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON-Parse-Fehler: {ex.Message}");
                Console.WriteLine($"Body that failed: '{body}'");
                return Results.BadRequest($"Invalid JSON: {ex.Message}");
            }

            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                Console.WriteLine("ERROR: Invalid request after parse");
                return Results.BadRequest("Missing email or password.");
            }

            // SignInManager: Validiert + setzt Cookie
            Console.WriteLine($"Attempting sign-in for: {request.Email}");
            var result = await signInManager.PasswordSignInAsync(
                request.Email,
                request.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                Console.WriteLine($"SUCCESS: Login succeeded for {request.Email}");
                // Cookie auto gesetzt via SignInManager (Response.Cookies.Append)
                return Results.Ok(new { message = "success" });
            }

            Console.WriteLine($"FAIL: Login failed for {request.Email} (Reason: {result}");
            return Results.Unauthorized();
        });
    }

    // LoginRequest (uppercase für Konsistenz, aber Parse ist case-insensitive)
    private record LoginRequest(string Email, string Password);
}