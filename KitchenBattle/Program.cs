using KitchenBattle.Data;
using KitchenBattle.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    options.InstanceName = "KitchenBattle_";
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RedisService>();
builder.Services.AddScoped<ScoreService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<BattleService>();
builder.Services.AddScoped<LeaderBoardService>();

builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IKeycloakAuthService, KeycloakAuthService>();

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