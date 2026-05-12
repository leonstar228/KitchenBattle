using KitchenBattle.Data;
using Microsoft.EntityFrameworkCore;
using KitchenBattle.Models;
using KitchenBattle.ViewModels;

namespace KitchenBattle.Services
{
    public class LeaderBoardService
    {
        private readonly ApplicationDbContext _db;

        public LeaderBoardService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<LeaderboardViewModel>> GetTopLeaderboard()
        {
            var topRecipe = await _db.Recipes
                .OrderByDescending(r => r.AverageScore)
                .Take(5).ToListAsync();
            
            var leaderboard = topRecipe
                .Select((r, index) => new LeaderboardViewModel
                {
                    RecipeId = r.Id,
                    RecipeTitle = r.Title,
                    ChefName = r.ChefName,
                    Category = r.Category.ToString(),
                    AverageScore = r.AverageScore,
                    TotalScoresCount = r.Scores.Count,
                    Place = index + 1
                })
                .ToList();

            return leaderboard;
        }
    }
}