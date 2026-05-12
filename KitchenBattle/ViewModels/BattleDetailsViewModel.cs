using KitchenBattle.Models;

namespace KitchenBattle.ViewModels
{
    public class BattleDetailsViewModel
    {
        public int Id { get; set; }
        public string BattleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public StatusBattleEnum Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public DateTime RegistrationStart { get; set; }
        public DateTime RegistrationEnd { get; set; }
        public CategoryEnum Category { get; set; }
        public string? WinnerId { get; set; }  // ← змінив на string
        public string? WinnerName { get; set; }
        
        public List<RecipeInBattleViewModel> Recipes { get; set; } = new();
        public List<UserInBattleViewModel> Chefs { get; set; } = new();
        public List<UserInBattleViewModel> Judges { get; set; } = new();
        
        public bool IsRegistrationOpen { get; set; }
        public bool CanRegisterAsChef { get; set; }
        public bool CanRegisterAsJudge { get; set; }
        public bool CanAddRecipe { get; set; }
    }
    
    public class RecipeInBattleViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ChefName { get; set; } = string.Empty;
        public double AverageScore { get; set; }
        public string? ImageUrl { get; set; }
    }
    
    public class UserInBattleViewModel
    {
        public string Id { get; set; } = string.Empty;  // ← string, бо IdentityUser.Id
        public string FullName { get; set; } = string.Empty;
    }
}