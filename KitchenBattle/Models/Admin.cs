using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace KitchenBattle.Models
{
    public class Admin : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}
