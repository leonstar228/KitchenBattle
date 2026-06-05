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
            var now = DateTime.UtcNow;
            var weekAgo = now.AddDays(-7);
            var twoWeeksAgo = now.AddDays(-14);

            var publishedRecipes = await _context.Recipes
                .Where(r => r.Status == StatusRecipeEnum.Published)
                .Select(r => new { r.AverageScore, r.CreatedAt })
                .ToListAsync();

            var globalAverage = publishedRecipes.Count > 0
                ? publishedRecipes.Average(r => r.AverageScore)
                : 0;

            var recentAvg = publishedRecipes
                .Where(r => r.CreatedAt >= weekAgo)
                .Select(r => r.AverageScore)
                .DefaultIfEmpty(0)
                .Average();

            var previousAvg = publishedRecipes
                .Where(r => r.CreatedAt >= twoWeeksAgo && r.CreatedAt < weekAgo)
                .Select(r => r.AverageScore)
                .DefaultIfEmpty(recentAvg)
                .Average();

            var weeklyPublications = new int[7];
            for (var i = 0; i < 7; i++)
            {
                var day = now.Date.AddDays(-(6 - i));
                var nextDay = day.AddDays(1);
                weeklyPublications[i] = await _context.Recipes.CountAsync(r =>
                    r.Status == StatusRecipeEnum.Published &&
                    r.CreatedAt >= day &&
                    r.CreatedAt < nextDay);
            }

            var pendingModeration = await _context.Recipes
                .Where(r => r.Status == StatusRecipeEnum.Checked)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .Select(r => new ModerationQueueItem
                {
                    Id = r.Id,
                    Title = r.Title,
                    ChefName = r.ChefName,
                    ImageUrl = r.ImageUrl,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            var recentBattles = await _context.Battles
                .Include(b => b.BattleChefs)
                .OrderByDescending(b => b.StartedAt)
                .Take(5)
                .Select(b => new ActiveBattleItem
                {
                    Id = b.Id,
                    BattleName = b.BattleName,
                    ParticipantCount = b.BattleChefs.Count,
                    Status = b.Status,
                    StartedAt = b.StartedAt,
                    EndedAt = b.EndedAt
                })
                .ToListAsync();

            return new AdminDashboardViewModel
            {
                TotalUsers = await _context.ApplicationUsers.CountAsync(),
                TotalChefs = await _context.BattleChefs.Select(bc => bc.ChefId).Distinct().CountAsync(),
                TotalJudges = await _context.BattleJudges.Select(bj => bj.JudgeId).Distinct().CountAsync(),
                TotalRecipes = await _context.Recipes.CountAsync(),
                TotalBattles = await _context.Battles.CountAsync(),

                PublishedRecipes = await _context.Recipes.CountAsync(r => r.Status == StatusRecipeEnum.Published),
                PendingRecipes = await _context.Recipes.CountAsync(r => r.Status == StatusRecipeEnum.Checked),
                RejectedRecipes = await _context.Recipes.CountAsync(r => r.Status == StatusRecipeEnum.Rejected),
                FinishedBattles = await _context.Battles.CountAsync(b => b.Status == StatusBattleEnum.Completed),
                ActiveBattles = await _context.Battles.CountAsync(b =>
                    b.Status == StatusBattleEnum.InProgress || b.Status == StatusBattleEnum.Closed),

                GlobalAverageScore = Math.Round(globalAverage, 1),
                ScoreWeeklyChange = Math.Round(recentAvg - previousAvg, 1),
                PendingModeration = pendingModeration,
                RecentBattles = recentBattles,
                WeeklyPublications = weeklyPublications
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