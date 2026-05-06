namespace KitchenBattle.Models
{
    public class BattleJudge
    {
        public int Id { get; set; }
        public int BattleId { get; set; }
        public string JudgeId { get; set; }

        public Battle Battle { get; set; }
        public Judge Judge { get; set; }
    }
}
