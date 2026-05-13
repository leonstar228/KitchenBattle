using KitchenBattle.Models;

namespace KitchenBattle.ViewModels
{
    public class LeaderboardViewModel
    {
        public int RecipeId { get; set; }
        public string RecipeTitle { get; set; } = string.Empty;
        public string ChefName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double AverageScore { get; set; }
        public int TotalScoresCount { get; set; }
        public int Place { get; set; }
    }
}
