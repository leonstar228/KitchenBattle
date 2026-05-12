using KitchenBattle.Models;

namespace KitchenBattle.ViewModels
{
    public class BattleCreateViewModel
    {
        public string BattleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public DateTime RegistrationStart { get; set; }
        public DateTime RegistrationEnd { get; set; }
        public CategoryEnum Category { get; set; }
    }
}