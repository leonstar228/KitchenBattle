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
            var recipeId = int.Parse(vm.RecipeId);
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
            
            var recipe = await _db.Recipes
                .Include(r => r.Scores)
                .FirstOrDefaultAsync(r => r.Id == recipeId);

            if (recipe != null)
            {
                recipe.AverageScore = recipe.Scores.Average(s => s.TotalScore);
                await _db.SaveChangesAsync();
            }
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
            await RecalculateAverageAsync(score.RecipeId);
        }
        
        public async Task DeleteScore(int id)
        {
            var score = await _db.Scores.FindAsync(id);
            if (score == null) return;

            var recipeId = score.RecipeId;
            _db.Scores.Remove(score);
            await _db.SaveChangesAsync();
            await RecalculateAverageAsync(recipeId);
        }
        
        public async Task<bool> CheckJudgeAlreadyScored(int recipeId, string judgeId)
        {
            return await _db.Scores
                .AnyAsync(s => s.RecipeId == recipeId && s.JudgeId == judgeId);
        }
        
        private async Task RecalculateAverageAsync(int recipeId)
        {
            var recipe = await _db.Recipes
                .Include(r => r.Scores)
                .FirstOrDefaultAsync(r => r.Id == recipeId);

            if (recipe == null) return;

            recipe.AverageScore = recipe.Scores.Any()
                ? recipe.Scores.Average(s => s.TotalScore)
                : 0;

            await _db.SaveChangesAsync();
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