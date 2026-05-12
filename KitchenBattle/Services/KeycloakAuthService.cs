using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;

namespace KitchenBattle.Services;

public class KeycloakAuthService : IKeycloakAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public KeycloakAuthService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public string GetUserId(ClaimsPrincipal user) => user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public string GetUsername(ClaimsPrincipal user) => user.FindFirst("preferred_username")?.Value ?? user.Identity?.Name;

    public bool IsAdmin(ClaimsPrincipal user) => user.IsInRole("admin");

    public bool IsChef(ClaimsPrincipal user) => user.IsInRole("chef");

    public bool IsJudge(ClaimsPrincipal user) => user.IsInRole("judge");

    public async Task LogoutAsync()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context == null) return;

        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
        {
            RedirectUri = "/"
        });
    }
}