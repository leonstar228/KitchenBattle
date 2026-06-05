using KitchenBattle.Models;

namespace KitchenBattle.ViewModels
{
    public class ModerationQueueItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ChefName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ActiveBattleItem
    {
        public int Id { get; set; }
        public string BattleName { get; set; } = string.Empty;
        public int ParticipantCount { get; set; }
        public StatusBattleEnum Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
    }
}
