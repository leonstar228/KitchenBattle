using Microsoft.EntityFrameworkCore;
using KitchenBattle.Data;
using KitchenBattle.Models;
using KitchenBattle.ViewModels;

namespace KitchenBattle.Services
{
    public interface IAdminService
    {
        Task<AdminDashboardViewModel> GetDashboardStatsAsync();
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task DeleteUserAsync(string userId);
        Task<Dictionary<CategoryEnum, double>> GetCategoryStatisticsAsync(); 
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
                TotalUsers = await _context.ApplicationUsers.CountAsync(),
                TotalChefs = await _context.BattleChefs.Select(bc => bc.ChefId).Distinct().CountAsync(),
                TotalJudges = await _context.BattleJudges.Select(bj => bj.JudgeId).Distinct().CountAsync(),
                TotalRecipes = await _context.Recipes.CountAsync(),
                TotalBattles = await _context.Battles.CountAsync(),

                PublishedRecipes = await _context.Recipes.CountAsync(r => r.Status == StatusRecipeEnum.Published),
                PendingRecipes = await _context.Recipes.CountAsync(r => r.Status == StatusRecipeEnum.Checked),
                FinishedBattles = await _context.Battles.CountAsync(b => b.Status == StatusBattleEnum.Completed)
            };
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _context.ApplicationUsers
                .OrderByDescending(u => u.UserName)
                .ToListAsync();
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _context.ApplicationUsers.FindAsync(userId);
            if (user != null)
            {
                _context.ApplicationUsers.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Dictionary<CategoryEnum, double>> GetCategoryStatisticsAsync()
        {
            return await _context.Recipes
                .Where(r => r.Status == StatusRecipeEnum.Published)
                .GroupBy(r => r.Category)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.Average(r => (double)r.AverageScore)
                );
        }
    }
}