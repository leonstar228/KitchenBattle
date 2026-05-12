using System.Security.Claims;

namespace KitchenBattle.Services
{
    public interface IKeycloakAuthService
    {
        string GetUserId(ClaimsPrincipal user);
        string GetUsername(ClaimsPrincipal user);
        bool IsAdmin(ClaimsPrincipal user);
        bool IsChef(ClaimsPrincipal user);
        bool IsJudge(ClaimsPrincipal user);
        Task LogoutAsync();
    }
}