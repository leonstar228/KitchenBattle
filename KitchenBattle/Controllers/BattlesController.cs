using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KitchenBattle.Models;
using KitchenBattle.ViewModels;
using KitchenBattle.Services;
using System.Security.Claims;
using KitchenBattle.Data;
using Microsoft.EntityFrameworkCore;

namespace KitchenBattle.Controllers
{
    public class BattlesController : Controller
    {
        private readonly BattleService _battleService;
        private readonly RedisService _redisService;
        private readonly ApplicationDbContext _context;

        public BattlesController(BattleService battleService, RedisService redisService, ApplicationDbContext context)
        {
            _battleService = battleService;
            _redisService = redisService;
            _context = context;
        }
        
        public async Task<IActionResult> Index(string status, string category, string sortBy, string search, int page = 1)
        {
            var battles = await _redisService.GetBattlesAsync();
            if (!string.IsNullOrEmpty(status))
            {
                battles = battles.Where(b => b.Status.ToString() == status).ToList();
            }

            if (!string.IsNullOrEmpty(category))
            {
                battles = battles.Where(b => b.Category.ToString() == category).ToList();
            }

            if (!string.IsNullOrEmpty(search))
            {
                battles = battles.Where(b => b.BattleName.Contains(search) || b.Description.Contains(search)).ToList();
            }

            battles = sortBy switch
            {
                "date" => battles.OrderByDescending(b => b.StartedAt).ToList(),
                "date_old" => battles.OrderBy(b => b.StartedAt).ToList(),
                "name" => battles.OrderBy(b => b.BattleName).ToList(),
                 _ => battles
            };
            int pageSize = 9;
            int total = (int)Math.Ceiling(battles.Count/ (double)pageSize);
            var pagedBattles = battles.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            ViewBag.CurrentPage = page;
            ViewBag.Totalpages = total;
            return View(pagedBattles);
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var battle = await _battleService.GetBattleByIdAsync(id);
            if (battle == null) return NotFound();
    
            ViewBag.PendingChefs = await _battleService.GetPendingChefsAsync(id);
            ViewBag.ApprovedChefs = await _battleService.GetApprovedChefsAsync(id);
            
            var chefId = User.FindFirstValue("sub") ?? 
                         User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                         User.Identity?.Name ?? "";
            
            var allChefRecipes = await _context.Recipes
                .Where(r => r.ChefId == chefId)
                .ToListAsync();
            
            var existingRecipeIds = battle.Recipes.Select(r => r.Id).ToHashSet();
            
            var chefRecipes = allChefRecipes.Where(r => !existingRecipeIds.Contains(r.Id)).ToList();
    
            ViewBag.ChefRecipes = chefRecipes;
    
            return View(battle);
        }
        
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View(new BattleCreateViewModel
            {
                StartedAt = DateTime.UtcNow.AddDays(7),
                RegistrationStart = DateTime.UtcNow,
                RegistrationEnd = DateTime.UtcNow.AddDays(7)
            });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(BattleCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.RegistrationStart >= model.RegistrationEnd)
            {
                ModelState.AddModelError("RegistrationEnd", "Кінець реєстрації має бути після початку");
                return View(model);
            }

            if (model.RegistrationEnd >= model.StartedAt)
            {
                ModelState.AddModelError("StartedAt", "Батл має початись після завершення реєстрації");
                return View(model);
            }

            await _battleService.CreateBattleAsync(model);

            TempData["Success"] = "Батл успішно створено!";
            return RedirectToAction(nameof(Index));
        }
        
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var battle = await _battleService.GetBattleByIdAsync(id);
            if (battle == null) return NotFound();

            var model = new BattleCreateViewModel
            {
                BattleName = battle.BattleName,
                Description = battle.Description,
                StartedAt = battle.StartedAt,
                EndedAt = battle.EndedAt,
                RegistrationStart = battle.RegistrationStart,
                RegistrationEnd = battle.RegistrationEnd,
                Category = battle.Category
            };

            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, BattleCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var battle = await _battleService.UpdateBattleAsync(id, model);
            if (battle == null) return NotFound();

            TempData["Success"] = "Батл оновлено!";
            return RedirectToAction(nameof(Details), new { id });
        }
        
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var battle = await _battleService.GetBattleByIdAsync(id);
            if (battle == null) return NotFound();

            return View(battle);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _battleService.DeleteBattleAsync(id);
            TempData["Success"] = "Батл видалено!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "chef")]
        public async Task<IActionResult> RegisterChef(int id)
        {
            var chefId = User.FindFirstValue("sub") ?? 
                         User.FindFirstValue(ClaimTypes.NameIdentifier) ??          
                         User.FindFirstValue("id") ??            
                         User.FindFirstValue("preferred_username") ?? 
                         User.Identity?.Name ?? "";
    
            Console.WriteLine($"ChefId used: {chefId}");
    
            var (success, message) = await _battleService.RegisterChefAsync(id, chefId);

            if (success) TempData["Success"] = message;
            else TempData["Error"] = message;

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "judge")]
        public async Task<IActionResult> RegisterJudge(int id)
        {
            var judgeId = User.FindFirstValue("sub") ?? 
                          User.FindFirstValue(ClaimTypes.NameIdentifier) ??         
                          User.FindFirstValue("id") ??          
                          User.FindFirstValue("preferred_username") ?? 
                          User.Identity?.Name ?? "";
    
            Console.WriteLine($"JudgeId used: {judgeId}");
    
            var (success, message) = await _battleService.RegisterJudgeAsync(id, judgeId);

            if (success) TempData["Success"] = message;
            else TempData["Error"] = message;

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ApproveChef(int battleId, string chefId)
        {
            var (success, message) = await _battleService.ApproveChefAsync(battleId, chefId);
            if (success) TempData["Success"] = message;
            else TempData["Error"] = message;
            return RedirectToAction(nameof(Details), new { id = battleId });
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> StartBattle(int id)
        {
            await _battleService.StartBattleAsync(id);
            TempData["Success"] = "Батл розпочато!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CloseRegistration(int id)
        {
            await _battleService.CloseRegistrationAsync(id);
            TempData["Success"] = "Реєстрацію закрито!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> FinishBattle(int id)
        {
            await _battleService.FinishBattleAsync(id);
            TempData["Success"] = "Батл завершено! Переможець визначений автоматично.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SetWinner(int id, string winnerChefId)
        {
            await _battleService.SetWinnerAsync(id, winnerChefId);
            TempData["Success"] = "Переможця встановлено!";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "chef")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddExistingRecipe(int battleId, int recipeId)
        {
            var chefId = User.FindFirstValue("sub") ?? 
                         User.FindFirstValue(ClaimTypes.NameIdentifier) ?? 
                         User.Identity?.Name ?? "";
            
            var battle = await _context.Battles
                .Include(b => b.Recipes)
                .Include(b => b.BattleChefs)
                .FirstOrDefaultAsync(b => b.Id == battleId);
            
            if (battle == null)
            {
                TempData["Error"] = "Батл не знайдено";
                return RedirectToAction(nameof(Details), new { id = battleId });
            }
            
            var isApproved = battle.BattleChefs.Any(bc => bc.ChefId == chefId && bc.IsApproved);
            if (!isApproved)
            {
                TempData["Error"] = "Ви не зареєстровані на цей батл";
                return RedirectToAction(nameof(Details), new { id = battleId });
            }
            
            if (battle.Status != StatusBattleEnum.InProgress)
            {
                TempData["Error"] = "Батл вже завершено або ще не почався";
                return RedirectToAction(nameof(Details), new { id = battleId });
            }
            
            var recipe = await _context.Recipes.FindAsync(recipeId);
            if (recipe == null || recipe.ChefId != chefId)
            {
                TempData["Error"] = "Рецепт не знайдено";
                return RedirectToAction(nameof(Details), new { id = battleId });
            }
            
            if (battle.Recipes.Any(r => r.Id == recipeId))
            {
                TempData["Error"] = "Цей рецепт вже додано до батлу";
                return RedirectToAction(nameof(Details), new { id = battleId });
            }
            
            battle.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Рецепт успішно додано до батлу!";
            return RedirectToAction(nameof(Details), new { id = battleId });
        }
    }
}