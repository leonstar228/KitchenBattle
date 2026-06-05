using KitchenBattle.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace KitchenBattle.ViewModels
{
    public class RecipeCreateViewModel
    {
        
        [StringLength(50, ErrorMessage = "Назва не може перевищувати 50 символів.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Назва не може перевищувати 50 символів.")]
        public string Description { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        
        [Range(3, 300, ErrorMessage = "Час приготування не може бути меншим за 3 хв та більшим ніж 300 хв.")]
        public int CookingTime { get; set; }
        public DifficultyEnum Difficulty { get; set; }
        public CategoryEnum Category { get; set; }
        public IFormFile? Picture { get; set; }
    }
}