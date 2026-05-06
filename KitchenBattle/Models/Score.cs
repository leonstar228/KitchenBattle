namespace KitchenBattle.Models
{
    public class Score
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string JudgeId { get; set; }
        public int Taste { get; set; }
        public int Presentation { get; set; }
        public int Creativity { get; set; }
        public int TotalScore => Taste + Presentation + Creativity;
        public string Comments { get; set; } = string.Empty;
    }
}
