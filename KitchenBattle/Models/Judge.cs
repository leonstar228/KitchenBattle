using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace KitchenBattle.Models
{
    public class Judge: IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Battle> Battles { get; set; } = new List<Battle>();
        public ICollection<BattleJudge> BattleJudges { get; set; } = new List<BattleJudge>();
    }
}
