using KitchenBattle.Data;
using KitchenBattle.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using KitchenBattle.ViewModels;

public class RedisService
{
    private readonly ApplicationDbContext _db;
    private readonly IDistributedCache _cache;

    public RedisService(ApplicationDbContext db, IDistributedCache cache)
    {
        _db = db;
        _cache = cache;
    }
    
    private const string RecipesCacheKey = "recipes_list";

    public async Task<List<Recipe>> GetRecipesAsync()
    {
        Console.WriteLine("Виклик GetRecipesAsync");
        var cached = await _cache.GetStringAsync(RecipesCacheKey);
        if (cached != null)
        {
            Console.WriteLine("Дані знайдено в кеші");
            return JsonSerializer.Deserialize<List<Recipe>>(cached) ?? new List<Recipe>();
        }
        Console.WriteLine("Кешь порожній!");
        var recipes = await _db.Recipes
            .OrderByDescending(r => r.Id)
            .ToListAsync();
        Console.WriteLine("Отримуємо данні з бази.");
        await _cache.SetStringAsync(RecipesCacheKey,
            JsonSerializer.Serialize(recipes),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        Console.WriteLine("Данні збережено в кеш.");
        return recipes;
    }
    
    private const string BattlesCacheKey = "battles_list";
    public async Task<List<Battle>> GetBattlesAsync()
    {
        Console.WriteLine("Виклик GetBattlesAsync");
        var cached = await _cache.GetStringAsync(BattlesCacheKey);
        if (cached != null)
        {
            Console.WriteLine("Данні знайдено в кеші!");
            return JsonSerializer.Deserialize<List<Battle>>(cached) ?? new List<Battle>();
        }
        Console.WriteLine("Кеш порожній!");
        var battles = await _db.Battles
            .OrderByDescending(b => b.StartedAt)
            .ToListAsync();
        Console.WriteLine("Отримуємо данні з бази.");
        await _cache.SetStringAsync(BattlesCacheKey,
            JsonSerializer.Serialize(battles),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        Console.WriteLine("Данні збережено в кеш.");
        return battles;
    }
    
    private const string ScoresCacheKey = "scores_list";

    public async Task<List<Score>> GetScoresAsync()
    {
        Console.WriteLine("Виклик GetScoresAsync");
        var cached = await _cache.GetStringAsync(ScoresCacheKey);
        if (cached != null)
        {
            Console.WriteLine("Данні знайдено в кеші!");
            return JsonSerializer.Deserialize<List<Score>>(cached) ?? new List<Score>();
        }
        Console.WriteLine("Кеш порожній!");
        var scores = await _db.Scores
            .OrderByDescending(s => s.TotalScore)
            .ToListAsync();
        Console.WriteLine("Отримуємо данні з бази.");
        await _cache.SetStringAsync(ScoresCacheKey,
            JsonSerializer.Serialize(scores),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        Console.WriteLine("Данні збережено в кеш.");
        return scores;
    }
    private const string LeaderboardsCacheKey = "leaderboards_list";
    public async Task<List<LeaderboardViewModel>> GetLeaderBoardCach()
    {
        var cached = await _cache.GetStringAsync(LeaderboardsCacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<List<LeaderboardViewModel>>(cached) ?? new List<LeaderboardViewModel>();
        }

        var topRecipe = await _db.Recipes
            .OrderByDescending(r => r.AverageScore)
            .Take(5).ToListAsync();
        
        var leaderboard = topRecipe
            .Select((r, index) => new LeaderboardViewModel
            {
                RecipeId = r.Id,
                RecipeTitle = r.Title,
                ChefName = r.ChefName,
                Category = r.Category.ToString(),
                AverageScore = r.AverageScore,
                TotalScoresCount = r.Scores.Count,
                Place = index + 1
            })
            .ToList();
        
        await _cache.SetStringAsync(LeaderboardsCacheKey,
            JsonSerializer.Serialize(leaderboard),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
        return leaderboard;
    }
    
    public static async Task ClearCacheAsync(IDistributedCache cache)
    {
        await cache.RemoveAsync(RecipesCacheKey);
        Console.WriteLine("Кеш рецептів очищено.");
        await cache.RemoveAsync(BattlesCacheKey);
        Console.WriteLine("Кеш батлів очищено.");
        await cache.RemoveAsync(ScoresCacheKey);
        Console.WriteLine("Кеш список рахунків очищено.");
    }

}
