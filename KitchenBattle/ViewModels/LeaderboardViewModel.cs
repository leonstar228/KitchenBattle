using KitchenBattle.Models;

namespace KitchenBattle.ViewModels
{
    public class LeaderboardViewModel
    {
        public List<Recipe> TopRecipes { get; set; } = new List<Recipe>();
        public List<ApplicationUser> TopChefs { get; set; } = new List<ApplicationUser>();
        public string? CurrentBattleName { get; set; }
    }
}
