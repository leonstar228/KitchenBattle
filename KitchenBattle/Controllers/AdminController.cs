using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KitchenBattle.Models;
using KitchenBattle.ViewModels;
using KitchenBattle.Services;

[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IRecipeService _recipeService;
    private readonly ICacheService _cacheService;

    public AdminController(IAdminService adminService, IRecipeService recipeService, ICacheService cacheService)
    {
        _adminService = adminService;
        _recipeService = recipeService;
        _cacheService = cacheService;
    }

    public async Task<IActionResult> Dashboard()
    {
        var stats = await _adminService.GetDashboardStatsAsync();
        return View(stats);
    }

    public async Task<IActionResult> RecipesForReview()
    {
        var recipes = await _recipeService.GetRecipesByStatusAsync(RecipeStatus.PendingReview);
        return View(recipes);
    }

    [HttpPost]
    public async Task<IActionResult> ApproveRecipe(int id)
    {
        await _recipeService.ApproveRecipeAsync(id);
        await _cacheService.RemoveAsync("leaderboard_main");
        return RedirectToAction(nameof(RecipesForReview));
    }

    [HttpPost]
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