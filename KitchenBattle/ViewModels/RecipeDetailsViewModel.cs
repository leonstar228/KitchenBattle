using KitchenBattle.Models;

namespace KitchenBattle.ViewModels
{
    public class RecipeDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public int CookingTime { get; set; }
        public DifficultyEnum Difficulty { get; set; }
        public CategoryEnum Category { get; set; }
        public string? ImageUrl { get; set; }
        public string ChefName { get; set; } = string.Empty;
        public int ChefId { get; set; }
        public StatusRecipeEnum Status { get; set; }
        public double AverageScore { get; set; }
        public List<ScoreDisplayViewModel> Scores { get; set; } = new();
        public bool CanScore { get; set; }
    }
}