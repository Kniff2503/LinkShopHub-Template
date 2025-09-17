using LinkShopHub.Infrastructure.Identity;
using LinkShopHub.Web.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace LinkShopHub.Web.Features.Account;

public static class AccountEndpoints
{
    public static void MapAccountApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/account")
                       .WithTags("Account")
                       .RequireAuthorization();

        group.MapPost("/change-password", ChangePassword);
        group.MapPost("/change-email", ChangeEmail);
        group.MapDelete("/delete", DeleteAccount);
        //group.MapPost("/avatar", UploadAvatar);
    }

    private static async Task<Results<Ok<Response>, BadRequest<Response>, UnauthorizedHttpResult>> ChangePassword(
        [FromBody] ChangePasswordRequest dto,
        UserManager<AppUser> userManager,
        ClaimsPrincipal user)
    {
        var appUser = await userManager.GetUserAsync(user);
        if (appUser is null) return Unauthorized();

        var result = await userManager.ChangePasswordAsync(appUser, dto.OldPassword, dto.NewPassword);
        return result.Succeeded
            ? Ok(new Response("Password changed successfully"))
            : BadRequest(new Response(result.Errors.Select(e => e.Description)));
    }

    private static async Task<Results<Ok<Response>, BadRequest<Response>, UnauthorizedHttpResult>> ChangeEmail(
        [FromBody] ChangeEmailRequest dto,
        UserManager<AppUser> userManager,
        IEmailService mail,
        ClaimsPrincipal user,
        HttpContext ctx)
    {
        var appUser = await userManager.GetUserAsync(user);
        if (appUser is null) return Unauthorized();

        var token = await userManager.GenerateChangeEmailTokenAsync(appUser, dto.NewEmail);
        var host = $"{ctx.Request.Scheme}://{ctx.Request.Host}";
        var callback = $"{host}/account/confirm-email-change" +
                       $"?userId={appUser.Id}" +
                       $"&newEmail={Uri.EscapeDataString(dto.NewEmail)}" +
                       $"&token={Uri.EscapeDataString(token)}";

        await mail.SendEmailChangeAsync(appUser.Email!, callback);
        return Ok(new Response("Check your inbox for the confirmation link."));
    }

    private static async Task<Results<Ok<Response>, BadRequest<Response>, UnauthorizedHttpResult>> DeleteAccount(
        [FromBody] DeleteAccountRequest dto,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ClaimsPrincipal user)
    {
        var appUser = await userManager.GetUserAsync(user);
        if (appUser is null) return Unauthorized();

        var correct = await userManager.CheckPasswordAsync(appUser, dto.Password);
        if (!correct)
            return BadRequest(new Response("Incorrect password"));

        await userManager.DeleteAsync(appUser);
        await signInManager.SignOutAsync();
        return Ok(new Response("Account deleted"));
    }

    //private static async Task<Results<Ok<Response>, BadRequest<Response>, UnauthorizedHttpResult>> UploadAvatar(
    //    IFormFile file,
    //    UserManager<AppUser> userManager,
    //    ClaimsPrincipal user,
    //    IWebHostEnvironment env)
    //{
    //    var appUser = await userManager.GetUserAsync(user);
    //    if (appUser is null) return Unauthorized();

    //    if (file.Length is 0 or > 2_000_000)
    //        return BadRequest(new Response("File empty or > 2 MB"));

    //    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
    //    if (!new[] { ".jpg", ".jpeg", ".png", ".webp" }.Contains(ext))
    //        return BadRequest(new Response("Only .jpg, .png, .webp allowed"));

    //    var fileName = $"{appUser.Id}{ext}";
    //    var folder = Path.Combine(env.WebRootPath, "avatars");
    //    Directory.CreateDirectory(folder);
    //    var path = Path.Combine(folder, fileName);

    //    await using var stream = File.OpenWrite(path);
    //    await file.CopyToAsync(stream);

    //    appUser.AvatarUrl = $"/avatars/{fileName}";
    //    await userManager.UpdateAsync(appUser);

    //    return Ok(new Response("Avatar uploaded"));
    //}

    /* ------------ DTOs ------------ */
    public record ChangePasswordRequest(string OldPassword, string NewPassword);
    public record ChangeEmailRequest(string NewEmail);
    public record DeleteAccountRequest(string Password);
    public record Response(string Message, IEnumerable<string>? Errors = null)
    {
        public Response(IEnumerable<string> errors) : this("Errors occurred", errors) { }
    }
}