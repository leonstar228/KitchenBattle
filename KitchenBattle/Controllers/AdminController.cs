using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KitchenBattle.Models;
using KitchenBattle.ViewModels;
using KitchenBattle.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace KitchenBattle.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IRecipeService _recipeService;
        private readonly RedisService _redisService;
        private readonly IDistributedCache _cache;

        public AdminController(
            IAdminService adminService,
            IRecipeService recipeService,
            RedisService redisService,
            IDistributedCache cache)
        {
            _adminService = adminService;
            _recipeService = recipeService;
            _redisService = redisService;
            _cache = cache;
        }

        public async Task<IActionResult> Dashboard()
        {
            var stats = await _adminService.GetDashboardStatsAsync();
            return View(stats);
        }

        public async Task<IActionResult> RecipesForReview()
        {
            var recipes = await _recipeService.GetRecipesByStatusAsync(StatusRecipeEnum.Checked);
            return View(recipes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRecipe(int id)
        {
            var success = await _recipeService.ApproveRecipeAsync(id);

            if (success)
            {
                await RedisService.ClearCacheAsync(_cache);
            }

            return RedirectToAction(nameof(RecipesForReview));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectRecipe(int id)
        {
            await _recipeService.RejectRecipeAsync(id);
            return RedirectToAction(nameof(RecipesForReview));
        }

        public async Task<IActionResult> Users()
        {
            var users = await _adminService.GetAllUsersAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _adminService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> Statistics()
        {
            var stats = await _adminService.GetCategoryStatisticsAsync();
            return View(stats);
        }
    }
}