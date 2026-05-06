using KitchenBattle.Models;
using Microsoft.AspNetCore.Http;

namespace KitchenBattle.ViewModels
{
    public class RecipeCreateViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public int CookingTime { get; set; }
        public DifficultyEnum Difficulty { get; set; }
        public CategoryEnum Category { get; set; }
        public IFormFile? Picture { get; set; }
    }
}