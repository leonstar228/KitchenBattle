namespace KitchenBattle.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
        public ICollection<Score> Scores { get; set; } = new List<Score>();
        public ICollection<BattleChef> BattleChefs { get; set; } = new List<BattleChef>();
        public ICollection<BattleJudge> BattleJudges { get; set; } = new List<BattleJudge>();
    }
}
