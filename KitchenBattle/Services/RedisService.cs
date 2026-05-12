using KitchenBattle.Data;
using KitchenBattle.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

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
        var cached = await _cache.GetStringAsync(RecipesCacheKey);
        if (cached != null)
            return JsonSerializer.Deserialize<List<Recipe>>(cached) ?? new List<Recipe>();

        var recipes = await _db.Recipes
            .OrderByDescending(r => r.Id)
            .ToListAsync();

        await _cache.SetStringAsync(RecipesCacheKey,
            JsonSerializer.Serialize(recipes),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        return recipes;
    }
    
    private const string BattlesCacheKey = "battles_list";
    public async Task<List<Battle>> GetBattlesAsync()
    {
        var cached = await _cache.GetStringAsync(BattlesCacheKey);
        if (cached != null)
            return JsonSerializer.Deserialize<List<Battle>>(cached) ?? new List<Battle>();

        var battles = await _db.Battles
            .OrderByDescending(b => b.StartedAt)
            .ToListAsync();

        await _cache.SetStringAsync(BattlesCacheKey,
            JsonSerializer.Serialize(battles),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        return battles;
    }
    
    private const string ScoresCacheKey = "scores_list";

    public async Task<List<Score>> GetScoresAsync()
    {
        var cached = await _cache.GetStringAsync(ScoresCacheKey);
        if (cached != null)
            return JsonSerializer.Deserialize<List<Score>>(cached) ?? new List<Score>();

        var scores = await _db.Scores
            .OrderByDescending(s => s.TotalScore)
            .ToListAsync();

        await _cache.SetStringAsync(ScoresCacheKey,
            JsonSerializer.Serialize(scores),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

        return scores;
    }
    
    public static async Task ClearCacheAsync(IDistributedCache cache)
    {
        await cache.RemoveAsync(RecipesCacheKey);
        await cache.RemoveAsync(BattlesCacheKey);
        await cache.RemoveAsync(ScoresCacheKey);
    }

}
