using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KitchenBattle.Models;
using KitchenBattle.ViewModels;
using KitchenBattle.Services;
using System.Security.Claims;

namespace KitchenBattle.Controllers
{
    public class BattlesController : Controller
    {
        private readonly BattleService _battleService;
        private readonly RedisService _redisService;

        public BattlesController(BattleService battleService, RedisService redisService)
        {
            _battleService = battleService;
            _redisService = redisService;
        }
        
        public async Task<IActionResult> Index()
        {
            var battles = await _redisService.GetBattlesAsync();
            return View(battles);
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var battle = await _battleService.GetBattleByIdAsync(id);
            if (battle == null) return NotFound();
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

        [HttpPost, ActionName("Delete")]
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
            var chefId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var (success, message) = await _battleService.RegisterChefAsync(id, chefId);

            if (success) TempData["Success"] = message;
            else TempData["Error"] = message;

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "judge")]
        public async Task<IActionResult> RegisterJudge(int id)
        {
            var judgeId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var (success, message) = await _battleService.RegisterJudgeAsync(id, judgeId);

            if (success) TempData["Success"] = message;
            else TempData["Error"] = message;

            return RedirectToAction(nameof(Details), new { id });
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
    }
}