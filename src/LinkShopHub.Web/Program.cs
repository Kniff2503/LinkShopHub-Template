using LinkShopHub.Infrastructure.Data;
using LinkShopHub.Infrastructure.Identity;
using LinkShopHub.Web.Components;
using LinkShopHub.Web.Data;
using LinkShopHub.Web.Features.Account;
using LinkShopHub.Web.Features.Auth;
using LinkShopHub.Web.Features.Billing;
using LinkShopHub.Web.Features.Health;
using LinkShopHub.Web.Features.Links;
using LinkShopHub.Web.Services;
using Mailjet.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- Datenbank (Business-DB) ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default"),
        npgsql => npgsql.EnableRetryOnFailure()));

// ---------- Identity-DB (separat) ----------
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Identity"),
        npgsql => npgsql.EnableRetryOnFailure()));

// ---------- Identity ----------
builder.Services
       .AddIdentity<AppUser, AppRole>(opts =>
       {
           opts.Password.RequireDigit = false;
           opts.Password.RequireNonAlphanumeric = false;
           opts.Password.RequireUppercase = false;
           opts.Password.RequiredLength = 6;
       })
       .AddEntityFrameworkStores<AppIdentityDbContext>()
       .AddDefaultTokenProviders();

// ---------- Cookie-Auth für Blazor Server ----------
builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // allow http
    opts.Cookie.SameSite = SameSiteMode.Lax; // only Development after strict
    opts.Cookie.HttpOnly = true;
    opts.SlidingExpiration = true;
    opts.ExpireTimeSpan = TimeSpan.FromDays(30);
});

// ---------- Blazor & Server-Side State ----------
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents();

// ---------- HTTP-Client (benannt) ----------
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7270/");
});

// ---------- Services ----------
builder.Services.AddScoped<IMailjetClient>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    return new MailjetClient(cfg["Mailjet:ApiKey"], cfg["Mailjet:SecretKey"]);
});
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<StripeCheckoutService>();

// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "LinkShopHub API", Version = "v1" }));

// ---------- Rate-Limiting ----------
builder.Services.AddRateLimiter(opts => opts
    .AddFixedWindowLimiter("global", cfg =>
    {
        cfg.PermitLimit = 100;
        cfg.Window = TimeSpan.FromMinutes(1);
        cfg.AutoReplenishment = true;
    }));

// ---------- MudBlazor ----------
builder.Services.AddMudServices();

var app = builder.Build();

// ---------- Development-Only ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LinkShopHub API v1"));

    using var scope = app.Services.CreateScope();
    var demo = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DemoSeed.SeedAsync(demo);
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// ---------- Pipeline ----------
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// ---------- Blazor ----------
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

// ---------- API-Module ----------
app.MapHealth();
app.MapLinkClicks();
app.MapBilling();
app.MapAuth();        // Login/Logout-Cookies
app.MapAccountApi();  // /api/account/*

app.Run();