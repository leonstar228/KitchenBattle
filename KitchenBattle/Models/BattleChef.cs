namespace KitchenBattle.Models
{
    public class BattleChef
    {
        public int Id { get; set; }
        public int BattleId { get; set; }
        public int ChefId { get; set; }

        // Навігаційні властивості
        public Battle Battle { get; set; }
        public Chef Chef { get; set; }
    }
}
