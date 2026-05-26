namespace KitchenBattle.Models
{
    public class BattleChef
    {
        public int Id { get; set; }
        public int BattleId { get; set; }
        public string ChefId { get; set; }
        
        public bool IsApproved { get; set; } = false;
        public Battle Battle { get; set; }
        public Chef Chef { get; set; }
    }
}
