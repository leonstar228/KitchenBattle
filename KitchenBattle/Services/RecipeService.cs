using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using KitchenBattle.Data;
using KitchenBattle.Models;
using KitchenBattle.ViewModels;
using System.Text.Json;

namespace KitchenBattle.Services
{
    public interface IRecipeService
    {
        Task<Recipe> CreateRecipeAsync(RecipeCreateViewModel model, string chefId, string chefName);
        Task<bool> UpdateRecipeAsync(RecipeEditViewModel model, string chefId);
        Task<bool> DeleteRecipeAsync(int recipeId, string chefId);
        Task<bool> SendToReviewAsync(int recipeId, string chefId);
        Task<bool> ApproveRecipeAsync(int recipeId);
        Task<bool> RejectRecipeAsync(int recipeId);
        Task<bool> RejectRecipeWithReasonAsync(int recipeId, string reason);
        Task<double> CalculateAverageScoreAsync(int recipeId);
        Task<Recipe?> GetRecipeByIdAsync(int recipeId);
        Task<List<Recipe>> GetRecipesByStatusAsync(StatusRecipeEnum status);
        Task<List<Recipe>> GetUserRecipesAsync(string chefId);
        Task<List<Recipe>> GetPublishedRecipesAsync();
    }

    public class RecipeService : IRecipeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDistributedCache _cache;

        private const string PublishedRecipesCacheKey = "published_recipes";
        private const string RecipeDetailsCacheKeyPrefix = "recipe_details_";
        private const string UserRecipesCacheKeyPrefix = "user_recipes_";

        public RecipeService(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IDistributedCache cache)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _cache = cache;
        }

        public async Task<List<Recipe>> GetPublishedRecipesAsync()
        {
            var cached = await _cache.GetStringAsync(PublishedRecipesCacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<List<Recipe>>(cached) ?? new List<Recipe>();

            var recipes = await _context.Recipes
                .Include(r => r.Scores)
                .Where(r => r.Status == StatusRecipeEnum.Published)
                .OrderByDescending(r => r.AverageScore)
                .ToListAsync();

            await _cache.SetStringAsync(PublishedRecipesCacheKey, JsonSerializer.Serialize(recipes),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

            return recipes;
        }

        public async Task<Recipe?> GetRecipeByIdAsync(int recipeId)
        {
            var cacheKey = $"{RecipeDetailsCacheKeyPrefix}{recipeId}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<Recipe>(cached);

            var recipe = await _context.Recipes.Include(r => r.Scores).FirstOrDefaultAsync(r => r.Id == recipeId);

            if (recipe != null)
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(recipe),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

            return recipe;
        }

        public async Task<List<Recipe>> GetUserRecipesAsync(string chefId)
        {
            var cacheKey = $"{UserRecipesCacheKeyPrefix}{chefId}";
            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<List<Recipe>>(cached) ?? new List<Recipe>();

            var recipes = await _context.Recipes.Include(r => r.Scores)
                .Where(r => r.ChefId == chefId).OrderByDescending(r => r.CreatedAt).ToListAsync();

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(recipes),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

            return recipes;
        }

        public async Task<Recipe> CreateRecipeAsync(RecipeCreateViewModel model, string chefId, string chefName)
        {
            string? imageUrl = null;
            if (model.Picture != null && model.Picture.Length > 0)
                imageUrl = await SaveImageAsync(model.Picture);

            var recipe = new Recipe
            {
                ChefId = chefId, ChefName = chefName, Title = model.Title,
                Description = model.Description, Ingredients = model.Ingredients,
                CookingTime = model.CookingTime, Difficulty = model.Difficulty,
                Category = model.Category, ImageUrl = imageUrl ?? string.Empty,
                Status = StatusRecipeEnum.Draft, AverageScore = 0,
                CreatedAt = DateTime.UtcNow, Scores = new List<Score>()
            };

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            await ClearRecipeCacheAsync(chefId);
            return recipe;
        }

        public async Task<bool> UpdateRecipeAsync(RecipeEditViewModel model, string chefId)
        {
            var recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.Id == model.Id && r.ChefId == chefId);
            if (recipe == null) return false;
            if (recipe.Status != StatusRecipeEnum.Draft && recipe.Status != StatusRecipeEnum.Rejected) return false;

            recipe.Title = model.Title; recipe.Description = model.Description;
            recipe.Ingredients = model.Ingredients; recipe.CookingTime = model.CookingTime;
            recipe.Difficulty = model.Difficulty; recipe.Category = model.Category;

            if (model.Picture != null && model.Picture.Length > 0)
            {
                if (!string.IsNullOrEmpty(recipe.ImageUrl)) DeleteImage(recipe.ImageUrl);
                recipe.ImageUrl = await SaveImageAsync(model.Picture);
            }

            await _context.SaveChangesAsync();
            await ClearRecipeCacheAsync(chefId, recipe.Id);
            return true;
        }

        public async Task<bool> DeleteRecipeAsync(int recipeId, string chefId)
        {
            var recipe = await _context.Recipes.Include(r => r.Scores)
                .FirstOrDefaultAsync(r => r.Id == recipeId && r.ChefId == chefId);
            if (recipe == null) return false;
            if (recipe.Status == StatusRecipeEnum.Published) return false;

            _context.Scores.RemoveRange(_context.Scores.Where(s => s.RecipeId == recipeId));
            if (!string.IsNullOrEmpty(recipe.ImageUrl)) DeleteImage(recipe.ImageUrl);

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            await ClearRecipeCacheAsync(chefId, recipeId);
            return true;
        }

        public async Task<bool> SendToReviewAsync(int recipeId, string chefId)
        {
            var recipe = await _context.Recipes.FirstOrDefaultAsync(r => r.Id == recipeId && r.ChefId == chefId);
            if (recipe == null || recipe.Status != StatusRecipeEnum.Draft) return false;

            recipe.Status = StatusRecipeEnum.Checked;
            recipe.RejectionReason = null;
            await _context.SaveChangesAsync();
            await ClearRecipeCacheAsync(chefId, recipeId);
            return true;
        }

        public async Task<bool> ApproveRecipeAsync(int recipeId)
        {
            var recipe = await _context.Recipes.FindAsync(recipeId);
            if (recipe == null || recipe.Status != StatusRecipeEnum.Checked) return false;

            recipe.Status = StatusRecipeEnum.Published;
            recipe.RejectionReason = null;
            await _context.SaveChangesAsync();
            await ClearRecipeCacheAsync(recipe.ChefId, recipeId);
            return true;
        }

        public async Task<bool> RejectRecipeAsync(int recipeId)
        {
            var recipe = await _context.Recipes.FindAsync(recipeId);
            if (recipe == null || recipe.Status != StatusRecipeEnum.Checked) return false;

            recipe.Status = StatusRecipeEnum.Rejected;
            await _context.SaveChangesAsync();
            await ClearRecipeCacheAsync(recipe.ChefId, recipeId);
            return true;
        }

        public async Task<bool> RejectRecipeWithReasonAsync(int recipeId, string reason)
        {
            var recipe = await _context.Recipes.FindAsync(recipeId);
            if (recipe == null || recipe.Status != StatusRecipeEnum.Checked) return false;

            recipe.Status = StatusRecipeEnum.Rejected;
            recipe.RejectionReason = reason?.Trim();
            await _context.SaveChangesAsync();
            await ClearRecipeCacheAsync(recipe.ChefId, recipeId);
            return true;
        }

        public async Task<double> CalculateAverageScoreAsync(int recipeId)
        {
            var scores = await _context.Scores.Where(s => s.RecipeId == recipeId).ToListAsync();
            if (!scores.Any()) return 0;

            var roundedAverage = Math.Round(scores.Average(s => s.TotalScore), 2);
            var recipe = await _context.Recipes.FindAsync(recipeId);
            if (recipe != null)
            {
                recipe.AverageScore = roundedAverage;
                await _context.SaveChangesAsync();
                await ClearRecipeCacheAsync(recipe.ChefId, recipeId);
            }
            return roundedAverage;
        }

        public async Task<List<Recipe>> GetRecipesByStatusAsync(StatusRecipeEnum status)
        {
            return await _context.Recipes.Include(r => r.Scores)
                .Where(r => r.Status == status).OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        private async Task ClearRecipeCacheAsync(string chefId, int? recipeId = null)
        {
            await _cache.RemoveAsync(PublishedRecipesCacheKey);
            if (recipeId.HasValue)
                await _cache.RemoveAsync($"{RecipeDetailsCacheKeyPrefix}{recipeId.Value}");
            await _cache.RemoveAsync($"{UserRecipesCacheKeyPrefix}{chefId}");
        }

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "recipes");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using var fileStream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(fileStream);
            return $"/uploads/recipes/{uniqueFileName}";
        }

        private void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            var filePath = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "recipes",
                System.IO.Path.GetFileName(imageUrl));
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
        }
    }
}
