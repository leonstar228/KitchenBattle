using Microsoft.EntityFrameworkCore;
using KitchenBattle.Data;
using KitchenBattle.Models;
using KitchenBattle.ViewModels;

namespace KitchenBattle.Services
{
    public class ScoreService
    {
        private readonly ApplicationDbContext _db;

        public ScoreService(ApplicationDbContext db)
        {
            _db = db;
        }
        
        public async Task<List<ScoreDisplayViewModel>> GetByRecipeIdAsync(int recipeId)
        {
            var scores = await _db.Scores
                .Where(s => s.RecipeId == recipeId)
                .ToListAsync();

            var result = new List<ScoreDisplayViewModel>();

            foreach (var s in scores)
            {
                var judge = await _db.Judges.FindAsync(s.JudgeId);

                result.Add(new ScoreDisplayViewModel
                {
                    Id = s.Id,
                    JudgeName = judge?.FullName ?? "Невідомий суддя",
                    Taste = s.Taste,
                    Presentation = s.Presentation,
                    Creativity = s.Creativity,
                    Comments = s.Comments
                });
            }

            return result;
        }
        
        public async Task AddScore(ScoreCreateViewModel vm, string judgeId)
        {
            var score = new Score
            {
                RecipeId = int.Parse(vm.RecipeId),
                JudgeId = judgeId,
                Taste = vm.Taste,
                Presentation = vm.Presentation,
                Creativity = vm.Creativity,
                Comments = vm.Comments
            };

            _db.Scores.Add(score);
            await _db.SaveChangesAsync();
        }
        
        public async Task<Score?> GetByIdAsync(int id)
        {
            return await _db.Scores.FindAsync(id);
        }
        
        public async Task UpdateScore(int id, ScoreCreateViewModel vm)
        {
            var score = await _db.Scores.FindAsync(id);
            if (score == null) return;

            score.Taste = vm.Taste;
            score.Presentation = vm.Presentation;
            score.Creativity = vm.Creativity;
            score.Comments = vm.Comments;

            await _db.SaveChangesAsync();
        }
        
        public async Task DeleteScore(int id)
        {
            var score = await _db.Scores.FindAsync(id);
            if (score == null) return;

            _db.Scores.Remove(score);
            await _db.SaveChangesAsync();
        }
        
        public async Task<bool> CheckJudgeAlreadyScored(int recipeId, string judgeId)
        {
            return await _db.Scores
                .AnyAsync(s => s.RecipeId == recipeId && s.JudgeId == judgeId);
        }
        
        public async Task<double> CalculateTotalScore(int recipeId)
        {
            var scores = await _db.Scores
                .Where(s => s.RecipeId == recipeId)
                .ToListAsync();

            if (!scores.Any()) return 0;

            return scores.Average(s => s.TotalScore);
        }
    }
}