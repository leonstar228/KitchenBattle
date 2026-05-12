namespace KitchenBattle.ViewModels
{
    public class ScoreDisplayViewModel
    {
        public int Id { get; set; }
        public string JudgeId { get; set; } = string.Empty;
        public string JudgeName { get; set; } = string.Empty;
        public int Taste { get; set; }
        public int Presentation { get; set; }
        public int Creativity { get; set; }
        public int TotalScore { get; set; }
        public string Comments { get; set; } = string.Empty;
    }
}