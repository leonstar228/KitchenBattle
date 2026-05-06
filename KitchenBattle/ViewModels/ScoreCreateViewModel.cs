using System.ComponentModel.DataAnnotations;
namespace KitchenBattle.ViewModels;

public class ScoreCreateViewModel
{
    public string RecipeId { get; set; } = string.Empty;
    [Required(ErrorMessage = "Оцінка смаку є обов'язковою.")]
    [Range(0, 10, ErrorMessage = "Оцінка смаку повинна бути від 0 до 10.")]
    public int Taste { get; set; }
    [Required(ErrorMessage = "Оцінка презентації є обов'язковою.")]
    [Range(0, 10, ErrorMessage = "Оцінка презентації повинна бути від 0 до 10.")]
    public int Presentation { get; set; }
    [Required(ErrorMessage = "Оцінка креативності є обов'язковою.")]
    [Range(0, 10, ErrorMessage = "Оцінка креативності повинна бути від 0 до 10.")]
    public int Creativity { get; set; }
    public string Comments { get; set; } = string.Empty;
}