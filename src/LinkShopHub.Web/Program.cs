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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"), npgsqlOptions =>
        npgsqlOptions.EnableRetryOnFailure()));

builder.Services.AddMudServices();
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7270/");
});

builder.Services.AddScoped<IMailjetClient>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    return new MailjetClient(cfg["Mailjet:ApiKey"], cfg["Mailjet:SecretKey"]);
});
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.AccessDeniedPath = "/auth/access-denied";
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<StripeCheckoutService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "LinkShopHub API", Version = "v1" });
});
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("global", config =>
    {
        config.PermitLimit = 100;
        config.Window = TimeSpan.FromMinutes(1);
        config.AutoReplenishment = true;
    });
});
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Identity")));

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppIdentityDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DemoSeed.SeedAsync(db);
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LinkShopHub API v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapBilling();
app.MapHealth();
app.MapLinkClicks();
app.MapAuth();
app.MapAccount();

app.Run();
