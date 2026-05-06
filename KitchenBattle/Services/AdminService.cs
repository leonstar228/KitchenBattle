using KitchenBattle.Models;
using Microsoft.EntityFrameworkCore;
using KitchenBattle.Models;
using KitchenBattle.ViewModels;
using KitchenBattle.Data;

namespace KitchenBattle.Services
{
    public interface IAdminService
    {
        Task<AdminDashboardViewModel> GetDashboardStatsAsync();
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task DeleteUserAsync(string userId);
        Task<Dictionary<RecipeCategory, double>> GetCategoryStatisticsAsync();
    }

    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardViewModel> GetDashboardStatsAsync()
        {
            return new AdminDashboardViewModel
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalChefs = await _context.BattleChefs.Select(bc => bc.ChefId).Distinct().CountAsync(),
                TotalJudges = await _context.BattleJudges.Select(bj => bj.JudgeId).Distinct().CountAsync(),
                TotalRecipes = await _context.Recipes.CountAsync(),
                TotalBattles = await _context.Battles.CountAsync(),
                PublishedRecipes = await _context.Recipes.CountAsync(r => r.StatusReciepe == RecipeStatus.Published),
                PendingRecipes = await _context.Recipes.CountAsync(r => r.StatusReciepe == RecipeStatus.PendingReview),
                FinishedBattles = await _context.Battles.CountAsync(b => b.StatusBattle == BattleStatus.Finished)
            };
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<RecipeCategory, double>> GetCategoryStatisticsAsync()
        {
            return await _context.Recipes
                .Where(r => r.StatusReciepe == RecipeStatus.Published)
                .GroupBy(r => r.Category)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Average(r => (double)r.AverageScore)
                );
        }
    }
}