using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KitchenBattle.Models;
using KitchenBattle.ViewModels;
using KitchenBattle.Data;
using KitchenBattle.Services;
using System.Security.Claims;

namespace KitchenBattle.Controllers
{
    [Authorize]
    public class RecipesController : Controller
    {
        private readonly IRecipeService _recipeService;
        private readonly ApplicationDbContext _context;

        public RecipesController(IRecipeService recipeService, ApplicationDbContext context)
        {
            _recipeService = recipeService;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(string? search, DifficultyEnum? difficulty, CategoryEnum? category)
        {
            var recipes = await _recipeService.GetPublishedRecipesAsync();

            if (!string.IsNullOrEmpty(search))
                recipes = recipes.Where(r => r.Title.Contains(search) || r.Description.Contains(search)).ToList();

            if (difficulty.HasValue)
                recipes = recipes.Where(r => r.Difficulty == difficulty.Value).ToList();

            if (category.HasValue)
                recipes = recipes.Where(r => r.Category == category.Value).ToList();

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentDifficulty = difficulty;
            ViewBag.CurrentCategory = category;

            return View(recipes);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (recipe == null) return NotFound();

            var currentUserId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var isJudge = User.IsInRole("judge");
            var hasScored = false;

            if (isJudge && !string.IsNullOrEmpty(currentUserId))
            {
                hasScored = await _context.Scores
                    .AnyAsync(s => s.RecipeId == id && s.JudgeId == currentUserId);
            }

            var scores = new List<ScoreDisplayViewModel>();
            foreach (var score in recipe.Scores)
            {
                var judge = await _context.Judges.FindAsync(score.JudgeId);
                scores.Add(new ScoreDisplayViewModel
                {
                    Id = score.Id,
                    JudgeName = judge?.FullName ?? "Unknown",
                    Taste = score.Taste,
                    Presentation = score.Presentation,
                    Creativity = score.Creativity,
                    Comments = score.Comments
                });
            }

            var isOwner = !string.IsNullOrEmpty(currentUserId) && recipe.ChefId == currentUserId;

            var viewModel = new RecipeDetailsViewModel
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Ingredients = recipe.Ingredients,
                CookingTime = recipe.CookingTime,
                Difficulty = recipe.Difficulty,
                Category = recipe.Category,
                ImageUrl = recipe.ImageUrl,
                ChefName = recipe.ChefName,
                ChefId = 0,
                Status = recipe.Status,
                AverageScore = recipe.AverageScore,
                Scores = scores,
                CanScore = isJudge && !hasScored && recipe.Status == StatusRecipeEnum.Published
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RecipeCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            
            var userId = User.FindFirstValue("sub") ?? 
                         User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                         User.Identity?.Name ?? "";
            
            var userName = User.FindFirstValue("preferred_username") ?? 
                           User.FindFirstValue("name") ?? 
                           User.Identity?.Name ?? "Unknown";

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            
            var chef = await _context.Chefs.FindAsync(userId);
            if (chef == null)
            {
                var fullName = User.FindFirstValue("name") ?? 
                               User.FindFirstValue("preferred_username") ?? 
                               userName;
        
                chef = new Chef
                {
                    Id = userId,
                    UserName = userName,
                    FullName = fullName,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Chefs.Add(chef);
                await _context.SaveChangesAsync();
            }

            await _recipeService.CreateRecipeAsync(model, userId, chef.FullName);

            return RedirectToAction(nameof(MyRecipes));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(r => r.Id == id && r.ChefId == userId);

            if (recipe == null) return NotFound();

            if (recipe.Status != StatusRecipeEnum.Draft && recipe.Status != StatusRecipeEnum.Rejected)
            {
                TempData["Error"] = "Не можна редагувати цей рецепт.";
                return RedirectToAction(nameof(MyRecipes));
            }

            var viewModel = new RecipeEditViewModel
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Ingredients = recipe.Ingredients,
                CookingTime = recipe.CookingTime,
                Difficulty = recipe.Difficulty,
                Category = recipe.Category,
                ExistingImageUrl = recipe.ImageUrl,
                Status = recipe.Status
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RecipeEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var success = await _recipeService.UpdateRecipeAsync(model, userId);

            if (!success)
            {
                TempData["Error"] = "Не вдалося оновити рецепт.";
                return View(model);
            }

            return RedirectToAction(nameof(MyRecipes));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(r => r.Id == id && r.ChefId == userId);

            if (recipe == null) return NotFound();

            if (recipe.Status == StatusRecipeEnum.Published)
            {
                TempData["Error"] = "Не можна видалити опублікований рецепт.";
                return RedirectToAction(nameof(MyRecipes));
            }

            return View(recipe);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var success = await _recipeService.DeleteRecipeAsync(id, userId);

            if (!success)
            {
                TempData["Error"] = "Не вдалося видалити рецепт.";
            }

            return RedirectToAction(nameof(MyRecipes));
        }

        public async Task<IActionResult> MyRecipes()
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var recipes = await _recipeService.GetUserRecipesAsync(userId);
            return View(recipes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendToReview(int id)
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var success = await _recipeService.SendToReviewAsync(id, userId);

            if (!success)
                TempData["Error"] = "Не вдалося відправити рецепт на перевірку.";

            return RedirectToAction(nameof(MyRecipes));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(int id)
        {
            var userId = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var success = await _recipeService.PublishRecipeAsync(id, userId);

            if (!success)
                TempData["Error"] = "Не вдалося опублікувати рецепт.";

            return RedirectToAction(nameof(MyRecipes));
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var success = await _recipeService.ApproveRecipeAsync(id);

            if (!success)
                TempData["Error"] = "Не вдалося схвалити рецепт.";

            return RedirectToAction("RecipesForReview", "Admin");
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var success = await _recipeService.RejectRecipeAsync(id);

            if (!success)
                TempData["Error"] = "Не вдалося відхилити рецепт.";

            return RedirectToAction("RecipesForReview", "Admin");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CalculateAverageScore(int id)
        {
            var average = await _recipeService.CalculateAverageScoreAsync(id);
            return Json(new { averageScore = average });
        }
    }
}