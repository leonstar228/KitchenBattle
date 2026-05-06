namespace KitchenBattle.Models
{
    public class Battle
    {
        public int Id { get; set; }
        public int WinnerId { get; set; }
        public string WinnerName { get; set; } = string.Empty;
        public StatusEnum Status { get; set; } = StatusEnum.Pending;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }
        public DateTime RegistrationStart { get; set; } = DateTime.UtcNow;
        public DateTime RegistrationEnd { get; set; } = DateTime.UtcNow.AddDays(7);
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
        public ICollection<BattleChef> BattleChefs { get; set; } = new List<BattleChef>();
        public ICollection<BattleJudge> BattleJudges { get; set; } = new List<BattleJudge>();
    }
}
