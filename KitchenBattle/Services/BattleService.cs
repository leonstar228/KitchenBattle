using KitchenBattle.Models;
using KitchenBattle.Data;
using KitchenBattle.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace KitchenBattle.Services
{
    public class BattleService
    {
        private readonly ApplicationDbContext _db;
        private readonly IDistributedCache _cache;  

        public BattleService(ApplicationDbContext db, IDistributedCache cache)
        {
            _db = db;
            _cache = cache;  
        }
        
        private async Task ClearCacheAsync()
        {
            await _cache.RemoveAsync("recipes_list");
            await _cache.RemoveAsync("battles_list");
            await _cache.RemoveAsync("scores_list");
        }

        public async Task<List<Battle>> GetAllBattlesAsync()
        {
            return await _db.Battles
                .Include(b => b.BattleChefs)
                .Include(b => b.BattleJudges)
                .OrderByDescending(b => b.StartedAt)
                .ToListAsync();
        }

        public async Task<Battle?> GetBattleByIdAsync(int id)
        {
            return await _db.Battles
                .Include(b => b.Recipes)
                    .ThenInclude(r => r.Scores)
                .Include(b => b.BattleChefs)
                    .ThenInclude(bc => bc.Chef)
                .Include(b => b.BattleJudges)
                    .ThenInclude(bj => bj.Judge)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Battle> CreateBattleAsync(BattleCreateViewModel model)
        {
            var battle = new Battle
            {
                BattleName = model.BattleName,
                Description = model.Description,
                StartedAt = model.StartedAt,
                EndedAt = model.EndedAt,
                RegistrationStart = model.RegistrationStart,
                RegistrationEnd = model.RegistrationEnd,
                Category = model.Category,
                Status = StatusBattleEnum.Pending,
                WinnerId = string.Empty
            };

            _db.Battles.Add(battle);
            await _db.SaveChangesAsync();

            await ClearCacheAsync();  

            return battle;
        }

        public async Task<Battle?> UpdateBattleAsync(int id, BattleCreateViewModel model)
        {
            var battle = await _db.Battles.FindAsync(id);
            if (battle == null) return null;

            battle.BattleName = model.BattleName;
            battle.Description = model.Description;
            battle.StartedAt = model.StartedAt;
            battle.EndedAt = model.EndedAt;
            battle.RegistrationStart = model.RegistrationStart;
            battle.RegistrationEnd = model.RegistrationEnd;
            battle.Category = model.Category;

            await _db.SaveChangesAsync();

            await ClearCacheAsync();  

            return battle;
        }

        public async Task<bool> DeleteBattleAsync(int id)
        {
            var battle = await _db.Battles
                .Include(b => b.BattleChefs)
                .Include(b => b.BattleJudges)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (battle == null) return false;

            _db.BattleChefs.RemoveRange(battle.BattleChefs);
            _db.BattleJudges.RemoveRange(battle.BattleJudges);
            _db.Battles.Remove(battle);
            await _db.SaveChangesAsync();

            await ClearCacheAsync();  

            return true;
        }
        
        public async Task<(bool success, string message)> RegisterChefAsync(int battleId, string chefId)
        {
            var battle = await _db.Battles
                .Include(b => b.BattleChefs)
                .FirstOrDefaultAsync(b => b.Id == battleId);

            if (battle == null)
                return (false, "Батл не знайдено");

            var now = DateTime.UtcNow;

            if (now < battle.RegistrationStart)
                return (false, $"Реєстрація почнеться {battle.RegistrationStart:dd.MM.yyyy HH:mm}");

            if (now > battle.RegistrationEnd)
                return (false, "Реєстрацію закрито");

            if (battle.Status != StatusBattleEnum.Pending)
                return (false, "Батл вже почався або завершився");

            if (battle.BattleChefs.Any(bc => bc.ChefId == chefId))
                return (false, "Ви вже зареєстровані");

            battle.BattleChefs.Add(new BattleChef { BattleId = battleId, ChefId = chefId });
            await _db.SaveChangesAsync();

            await ClearCacheAsync(); 

            return (true, "Ви успішно зареєстровані на батл!");
        }

        public async Task<(bool success, string message)> RegisterJudgeAsync(int battleId, string judgeId)
        {
            var battle = await _db.Battles
                .Include(b => b.BattleJudges)
                .FirstOrDefaultAsync(b => b.Id == battleId);

            if (battle == null)
                return (false, "Батл не знайдено");

            var now = DateTime.UtcNow;

            if (now < battle.RegistrationStart)
                return (false, $"Реєстрація почнеться {battle.RegistrationStart:dd.MM.yyyy HH:mm}");

            if (now > battle.RegistrationEnd)
                return (false, "Реєстрацію закрито");

            if (battle.Status != StatusBattleEnum.Pending)
                return (false, "Батл вже почався або завершився");

            if (battle.BattleJudges.Any(bj => bj.JudgeId == judgeId))
                return (false, "Суддя вже зареєстрований");

            battle.BattleJudges.Add(new BattleJudge { BattleId = battleId, JudgeId = judgeId });
            await _db.SaveChangesAsync();

            await ClearCacheAsync();  

            return (true, "Суддю успішно зареєстровано!");
        }

        public async Task<bool> CloseRegistrationAsync(int battleId)
        {
            var battle = await _db.Battles.FindAsync(battleId);
            if (battle == null) return false;

            if (battle.Status != StatusBattleEnum.Pending)
                return false;

            battle.RegistrationEnd = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await ClearCacheAsync();  

            return true;
        }

        public async Task<bool> StartBattleAsync(int battleId)
        {
            var battle = await _db.Battles.FindAsync(battleId);
            if (battle == null) return false;

            if (battle.Status != StatusBattleEnum.Pending)
                return false;

            battle.Status = StatusBattleEnum.InProgress;
            battle.StartedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await ClearCacheAsync();  

            return true;
        }

        public async Task<bool> FinishBattleAsync(int battleId)
        {
            var battle = await _db.Battles
                .Include(b => b.Recipes)
                    .ThenInclude(r => r.Scores)
                .FirstOrDefaultAsync(b => b.Id == battleId);

            if (battle == null) return false;

            if (battle.Status != StatusBattleEnum.InProgress)
                return false;

            battle.Status = StatusBattleEnum.Completed;
            battle.EndedAt = DateTime.UtcNow;

            if (battle.Recipes.Any())
            {
                var winnerRecipe = battle.Recipes
                    .OrderByDescending(r => r.AverageScore)
                    .FirstOrDefault();

                if (winnerRecipe != null)
                {
                    battle.WinnerId = winnerRecipe.ChefId;
                }
            }

            await _db.SaveChangesAsync();

            await ClearCacheAsync();  

            return true;
        }

        public async Task<bool> SetWinnerAsync(int battleId, string winnerChefId)
        {
            var battle = await _db.Battles.FindAsync(battleId);
            if (battle == null) return false;

            if (battle.Status != StatusBattleEnum.Completed)
                return false;

            battle.WinnerId = winnerChefId;
            await _db.SaveChangesAsync();

            await ClearCacheAsync(); 

            return true;
        }
    }
}