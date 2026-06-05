using System.ComponentModel.DataAnnotations;

namespace KitchenBattle.Models
{
    public class Judge
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(256)]
        public string? Email { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Battle> Battles { get; set; } = new List<Battle>();

        public ICollection<BattleJudge> BattleJudges { get; set; } = new List<BattleJudge>();
    }
}
