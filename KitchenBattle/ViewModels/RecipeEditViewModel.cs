using KitchenBattle.Models;
using Microsoft.AspNetCore.Http;

namespace KitchenBattle.ViewModels
{
    public class RecipeEditViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public int CookingTime { get; set; }
        public DifficultyEnum Difficulty { get; set; }
        public CategoryEnum Category { get; set; }
        public IFormFile? Picture { get; set; }
        public string? ExistingImageUrl { get; set; }
        public StatusRecipeEnum Status { get; set; }
    }
}