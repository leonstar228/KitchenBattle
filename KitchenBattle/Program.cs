using KitchenBattle.Data;
using KitchenBattle.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    options.InstanceName = "KitchenBattle_";
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    options.Authority = "http://localhost:8080/realms/kitchenbattle";
    options.ClientId = "kitchenbattle-web";
    options.ClientSecret = "KIgXjxOt25Cmrb1ZsqH6NQ5t2neGnCVM";
    options.ResponseType = "code";
    options.RequireHttpsMetadata = false;
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        NameClaimType = "preferred_username",
        // Use the standard role claim type so ASP.NET Core can evaluate roles from the added claims
        RoleClaimType = ClaimTypes.Role
    };

    // Extract roles that Keycloak may provide inside complex claims like "realm_access" or "resource_access"
    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = context =>
        {
            var identity = context.Principal?.Identity as ClaimsIdentity;
            if (identity == null) return Task.CompletedTask;

            // realm_access: { "roles": [ ... ] }
            var realmAccess = context.Principal.FindFirst("realm_access");
            if (realmAccess != null)
            {
                try
                {
                    using var doc = JsonDocument.Parse(realmAccess.Value);
                    if (doc.RootElement.TryGetProperty("roles", out var roles))
                    {
                        foreach (var r in roles.EnumerateArray())
                        {
                            var role = r.GetString();
                            if (!string.IsNullOrEmpty(role))
                                identity.AddClaim(new Claim(identity.RoleClaimType ?? ClaimTypes.Role, role));
                        }
                    }
                }
                catch { }
            }

            // resource_access: { "client-id": { "roles": [ ... ] }, ... }
            var resourceAccess = context.Principal.FindFirst("resource_access");
            if (resourceAccess != null)
            {
                try
                {
                    using var doc = JsonDocument.Parse(resourceAccess.Value);
                    foreach (var client in doc.RootElement.EnumerateObject())
                    {
                        if (client.Value.TryGetProperty("roles", out var roles))
                        {
                            foreach (var r in roles.EnumerateArray())
                            {
                                var role = r.GetString();
                                if (!string.IsNullOrEmpty(role))
                                    identity.AddClaim(new Claim(identity.RoleClaimType ?? ClaimTypes.Role, role));
                            }
                        }
                    }
                }
                catch { }
            }

            return Task.CompletedTask;
        }
    };

});

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RedisService>();
builder.Services.AddScoped<ScoreService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<BattleService>();
builder.Services.AddScoped<LeaderBoardService>();

builder.Services.AddScoped<RecipeService>();
builder.Services.AddScoped<KeycloakAuthService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();