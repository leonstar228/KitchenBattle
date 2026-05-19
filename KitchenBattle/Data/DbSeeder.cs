using KitchenBattle.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace KitchenBattle.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (HasAny(context, typeof(Recipe)) || HasAny(context, typeof(Battle)))
            return;

        var chefs = new List<Chef>();
        var judges = new List<Judge>();
        var admins = new List<Admin>();

        for (int i = 1; i <= 15; i++)
        {
            var chef = new Chef();
            Set(chef, "FullName", $"Шеф-кухар {i}");
            Set(chef, "Name", $"Шеф-кухар {i}");
            Set(chef, "Email", $"chef{i}@kitchenbattle.com");
            Set(chef, "CreatedAt", DateTime.Now.AddDays(-60 + i));
            chefs.Add(chef);
            context.Add(chef);
        }

        for (int i = 1; i <= 8; i++)
        {
            var judge = new Judge();
            Set(judge, "FullName", $"Суддя {i}");
            Set(judge, "Name", $"Суддя {i}");
            Set(judge, "Email", $"judge{i}@kitchenbattle.com");
            Set(judge, "CreatedAt", DateTime.Now.AddDays(-50 + i));
            judges.Add(judge);
            context.Add(judge);
        }

        for (int i = 1; i <= 3; i++)
        {
            var admin = new Admin();
            Set(admin, "FullName", $"Адміністратор {i}");
            Set(admin, "Name", $"Адміністратор {i}");
            Set(admin, "Email", $"admin{i}@kitchenbattle.com");
            Set(admin, "CreatedAt", DateTime.Now.AddDays(-70 + i));
            admins.Add(admin);
            context.Add(admin);
        }

        await context.SaveChangesAsync();

        var battles = new List<Battle>
        {
            MakeBattle("Весняна битва десертів", "Dessert", "Finished", -45, -44, -50, -46),
            MakeBattle("Італійська кухня", "Italian", "Finished", -30, -29, -35, -31),
            MakeBattle("Український смак", "Ukrainian", "Finished", -20, -19, -25, -21),
            MakeBattle("Гриль-челендж", "Grill", "InProgress", -1, 1, -5, -1),
            MakeBattle("Битва сніданків", "Breakfast", "Await", 5, 6, 1, 4),
            MakeBattle("Ресторанна подача", "Restaurant", "Await", 12, 13, 6, 11),
            MakeBattle("Вегетаріанський конкурс", "Vegetarian", "Await", 20, 21, 14, 19),
            MakeBattle("Фінальна битва шефів", "MainDish", "Await", 30, 31, 22, 29)
        };

        foreach (var battle in battles)
            context.Add(battle);

        await context.SaveChangesAsync();

        var recipeTitles = new[]
        {
            "Медовий торт з карамеллю", "Паста карбонара", "Борщ з пампушками", "Стейк з овочами гриль",
            "Сирники з ягідним соусом", "Різото з грибами", "Вареники з картоплею", "Курка теріякі",
            "Шоколадний фондан", "Цезар з куркою", "Лазанья болоньєзе", "Крем-суп з гарбуза",
            "Домашня піца", "Котлета по-київськи", "Тірамісу", "Філе лосося",
            "Панкейки з бананом", "Рамен з куркою", "Голубці у томатному соусі", "Брускета з томатами",
            "Чізкейк Нью-Йорк", "Плов з яловичиною", "Салат з авокадо", "Качина грудка",
            "Круасани з шоколадом", "Тако з куркою", "Овочеве рагу", "Паста з морепродуктами",
            "Картопляний гратен", "Млинці з сиром"
        };

        var recipes = new List<Recipe>();

        for (int i = 0; i < recipeTitles.Length; i++)
        {
            var chef = chefs[i % chefs.Count];
            var battle = battles[i % battles.Count];

            var recipe = new Recipe();

            Set(recipe, "Title", recipeTitles[i]);
            Set(recipe, "Description", $"Авторська страва для конкурсу Kitchen Battle. Страва має продуману подачу, збалансований смак і цікаву техніку приготування.");
            Set(recipe, "Ingredients", "Основні інгредієнти: овочі, спеції, соус, основний білковий продукт, зелень, гарнір.");
            Set(recipe, "CookingTime", 25 + i * 3);
            Set(recipe, "Difficulty", i % 3 == 0 ? "Easy" : i % 3 == 1 ? "Medium" : "Hard");
            Set(recipe, "Status", i % 5 == 0 ? "Draft" : i % 4 == 0 ? "Pending" : "Published");
            Set(recipe, "ChefName", GetString(chef, "FullName") ?? GetString(chef, "Name") ?? $"Шеф-кухар {(i % chefs.Count) + 1}");
            Set(recipe, "ChefId", GetId(chef));
            Set(recipe, "BattleId", GetId(battle));
            Set(recipe, "CreatedAt", DateTime.Now.AddDays(-40 + i));
            Set(recipe, "AverageScore", 0);

            recipes.Add(recipe);
            context.Add(recipe);
        }

        await context.SaveChangesAsync();

        var random = new Random(10);

        foreach (var recipe in recipes.Where((_, index) => index % 5 != 0))
        {
            var judgeCount = random.Next(3, 6);
            var selectedJudges = judges.OrderBy(_ => random.Next()).Take(judgeCount).ToList();

            foreach (var judge in selectedJudges)
            {
                var taste = random.Next(6, 11);
                var presentation = random.Next(6, 11);
                var creativity = random.Next(6, 11);

                var score = new Score();

                Set(score, "RecipeId", GetId(recipe));
                Set(score, "JudgeId", GetId(judge));
                Set(score, "JudgeName", GetString(judge, "FullName") ?? GetString(judge, "Name") ?? "Суддя");
                Set(score, "Taste", taste);
                Set(score, "Presentation", presentation);
                Set(score, "Creativity", creativity);
                Set(score, "TotalScore", taste + presentation + creativity);
                Set(score, "Comment", MakeComment(taste + presentation + creativity));
                Set(score, "CreatedAt", DateTime.Now.AddDays(-20 + random.Next(1, 15)));

                context.Add(score);
            }
        }

        await context.SaveChangesAsync();

        foreach (var recipe in recipes)
        {
            var recipeId = GetId(recipe);
            var scores = context.Scores
                .AsEnumerable()
                .Where(s => EqualsValue(GetValue(s, "RecipeId"), recipeId))
                .Select(s => Convert.ToDouble(GetValue(s, "TotalScore") ?? 0))
                .ToList();

            if (scores.Any())
                Set(recipe, "AverageScore", Math.Round(scores.Average(), 2));
        }

        await context.SaveChangesAsync();

        foreach (var battle in battles)
        {
            foreach (var chef in chefs.OrderBy(_ => random.Next()).Take(5))
            {
                AddJoinIfExists(context, typeof(BattleChef), battle, chef);
            }

            foreach (var judge in judges.OrderBy(_ => random.Next()).Take(3))
            {
                AddJoinIfExists(context, typeof(BattleJudge), battle, judge);
            }
        }

        await context.SaveChangesAsync();
    }

    private static Battle MakeBattle(string title, string category, string status, int startOffset, int endOffset, int regStartOffset, int regEndOffset)
    {
        var battle = new Battle();

        Set(battle, "Title", title);
        Set(battle, "Name", title);
        Set(battle, "Description", $"Кулінарний батл: {title}. Учасники готують авторські страви, а журі оцінює смак, подачу та креативність.");
        Set(battle, "Category", category);
        Set(battle, "Status", status);
        Set(battle, "StartAt", DateTime.Now.AddDays(startOffset).Date.AddHours(12));
        Set(battle, "EndAt", DateTime.Now.AddDays(endOffset).Date.AddHours(18));
        Set(battle, "RegistrationStart", DateTime.Now.AddDays(regStartOffset).Date.AddHours(9));
        Set(battle, "RegistrationEnd", DateTime.Now.AddDays(regEndOffset).Date.AddHours(23));
        Set(battle, "CreatedAt", DateTime.Now.AddDays(regStartOffset - 2));
        Set(battle, "WinnerId", Guid.NewGuid().ToString());
        Set(battle, "JudgeId", Guid.NewGuid().ToString());

        return battle;
    }

    private static string MakeComment(int total)
    {
        if (total >= 28) return "Відмінна страва, гарна подача та дуже збалансований смак.";
        if (total >= 24) return "Сильна робота, є кілька дрібних моментів для покращення.";
        if (total >= 20) return "Непогана страва, але потрібно допрацювати смак або презентацію.";
        return "Ідея цікава, але виконання потребує значного покращення.";
    }

    private static void AddJoinIfExists(ApplicationDbContext context, Type joinType, object battle, object user)
    {
        if (context.Model.FindEntityType(joinType) == null)
            return;

        var entity = Activator.CreateInstance(joinType);
        if (entity == null)
            return;

        Set(entity, "BattleId", GetId(battle));
        Set(entity, "ChefId", GetId(user));
        Set(entity, "JudgeId", GetId(user));

        context.Add(entity);
    }

    private static bool HasAny(ApplicationDbContext context, Type type)
    {
        var property = context.GetType()
            .GetProperties()
            .FirstOrDefault(p =>
                p.PropertyType.IsGenericType &&
                p.PropertyType.GetGenericArguments().FirstOrDefault() == type);

        if (property?.GetValue(context) is not IEnumerable values)
            return false;

        return values.Cast<object>().Any();
    }

    private static void Set(object obj, string propertyName, object? value)
    {
        var property = obj.GetType().GetProperty(propertyName);
        if (property == null || !property.CanWrite || value == null)
            return;

        var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

        try
        {
            if (targetType.IsEnum)
            {
                if (value is string text)
                {
                    if (Enum.TryParse(targetType, text, true, out var parsed))
                    {
                        property.SetValue(obj, parsed);
                        return;
                    }

                    property.SetValue(obj, Enum.GetValues(targetType).GetValue(0));
                    return;
                }

                property.SetValue(obj, Enum.ToObject(targetType, value));
                return;
            }

            if (targetType == typeof(string))
            {
                property.SetValue(obj, value.ToString());
                return;
            }

            if (targetType == typeof(DateTime))
            {
                property.SetValue(obj, Convert.ToDateTime(value));
                return;
            }

            property.SetValue(obj, Convert.ChangeType(value, targetType));
        }
        catch
        {
        }
    }

    private static object? GetId(object obj)
    {
        return GetValue(obj, "Id");
    }

    private static object? GetValue(object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
    }

    private static string? GetString(object obj, string propertyName)
    {
        return GetValue(obj, propertyName)?.ToString();
    }

    private static bool EqualsValue(object? a, object? b)
    {
        if (a == null || b == null)
            return false;

        return a.ToString() == b.ToString();
    }
}