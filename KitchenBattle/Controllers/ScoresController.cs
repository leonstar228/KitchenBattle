using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using KitchenBattle.Services;
using KitchenBattle.ViewModels;

namespace KitchenBattle.Controllers
{
    public class ScoresController : Controller
    {
        private readonly ScoreService _scoreService;

        public ScoresController(ScoreService scoreService)
        {
            _scoreService = scoreService;
        }
        
        public async Task<IActionResult> RecipeScores(int recipeId)
        {
            var scores = await _scoreService.GetByRecipeIdAsync(recipeId);
            ViewBag.RecipeId = recipeId;
            return View(scores);
        }
        
        [Authorize(Roles = "judge, admin")]
        public IActionResult Create(int recipeId)
        {
            var vm = new ScoreCreateViewModel
            {
                RecipeId = recipeId.ToString()
            };
            return View(vm);
        }
        
        [HttpPost]
        [Authorize(Roles = "judge, admin")]
        public async Task<IActionResult> Create(ScoreCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var judgeId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (judgeId == null)
                return Unauthorized();

            var recipeId = int.Parse(vm.RecipeId);

            var alreadyScored = await _scoreService.CheckJudgeAlreadyScored(recipeId, judgeId);
            if (alreadyScored)
            {
                ModelState.AddModelError("", "Ви вже оцінили цей рецепт.");
                return View(vm);
            }

            await _scoreService.AddScore(vm, judgeId);
            return RedirectToAction("Details", "Recipes", new { id = recipeId });
        }
        
        [Authorize(Roles = "judge, admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var score = await _scoreService.GetByIdAsync(id);
            if (score == null) return NotFound();

            var vm = new ScoreCreateViewModel
            {
                RecipeId = score.RecipeId.ToString(),
                Taste = score.Taste,
                Presentation = score.Presentation,
                Creativity = score.Creativity,
                Comments = score.Comments
            };

            return View(vm);
        }
        
        [HttpPost]
        [Authorize(Roles = "judge, admin")]
        public async Task<IActionResult> Edit(int id, ScoreCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            await _scoreService.UpdateScore(id, vm);
            return RedirectToAction("RecipeScores", new { recipeId = int.Parse(vm.RecipeId) });
        }
        
        [HttpPost]
        [Authorize(Roles = "judge, admin")]
        public async Task<IActionResult> Delete(int id, int recipeId)
        {
            await _scoreService.DeleteScore(id);
            return RedirectToAction("RecipeScores", new { recipeId });
        }
    }
}