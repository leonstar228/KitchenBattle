using KitchenBattle.Models;
using Microsoft.EntityFrameworkCore;

namespace KitchenBattle.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Battles.AnyAsync() || await context.Recipes.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var admins = new List<Admin>
        {
            CreateAdmin("admin-001", "admin.marta", "Марта Коваленко", "marta.admin@kitchenbattle.com", now.AddDays(-120)),
            CreateAdmin("admin-002", "admin.oleksandr", "Олександр Романюк", "oleksandr.admin@kitchenbattle.com", now.AddDays(-110)),
            CreateAdmin("admin-003", "admin.iryna", "Ірина Бойко", "iryna.admin@kitchenbattle.com", now.AddDays(-95))
        };

        var chefs = new List<Chef>
        {
            CreateChef("chef-001", "chef.andrii", "Андрій Мельник", "andrii.chef@kitchenbattle.com", now.AddDays(-90)),
            CreateChef("chef-002", "chef.olena", "Олена Шевчук", "olena.chef@kitchenbattle.com", now.AddDays(-88)),
            CreateChef("chef-003", "chef.nazar", "Назар Кравець", "nazar.chef@kitchenbattle.com", now.AddDays(-86)),
            CreateChef("chef-004", "chef.sofiia", "Софія Данилюк", "sofiia.chef@kitchenbattle.com", now.AddDays(-84)),
            CreateChef("chef-005", "chef.maksym", "Максим Савчук", "maksym.chef@kitchenbattle.com", now.AddDays(-82)),
            CreateChef("chef-006", "chef.kateryna", "Катерина Мороз", "kateryna.chef@kitchenbattle.com", now.AddDays(-80)),
            CreateChef("chef-007", "chef.roman", "Роман Литвин", "roman.chef@kitchenbattle.com", now.AddDays(-78)),
            CreateChef("chef-008", "chef.viktoriia", "Вікторія Гнатюк", "viktoriia.chef@kitchenbattle.com", now.AddDays(-76)),
            CreateChef("chef-009", "chef.dmytro", "Дмитро Федорів", "dmytro.chef@kitchenbattle.com", now.AddDays(-74)),
            CreateChef("chef-010", "chef.maryna", "Марина Ткачук", "maryna.chef@kitchenbattle.com", now.AddDays(-72)),
            CreateChef("chef-011", "chef.bohdan", "Богдан Павлюк", "bohdan.chef@kitchenbattle.com", now.AddDays(-70)),
            CreateChef("chef-012", "chef.anastasiia", "Анастасія Клим", "anastasiia.chef@kitchenbattle.com", now.AddDays(-68)),
            CreateChef("chef-013", "chef.yurii", "Юрій Стеценко", "yurii.chef@kitchenbattle.com", now.AddDays(-66)),
            CreateChef("chef-014", "chef.tetiana", "Тетяна Верес", "tetiana.chef@kitchenbattle.com", now.AddDays(-64)),
            CreateChef("chef-015", "chef.ihor", "Ігор Кушнір", "ihor.chef@kitchenbattle.com", now.AddDays(-62)),
            CreateChef("chef-016", "chef.alina", "Аліна Руденко", "alina.chef@kitchenbattle.com", now.AddDays(-60)),
            CreateChef("chef-017", "chef.volodymyr", "Володимир Сливка", "volodymyr.chef@kitchenbattle.com", now.AddDays(-58)),
            CreateChef("chef-018", "chef.nataliia", "Наталія Пилипчук", "nataliia.chef@kitchenbattle.com", now.AddDays(-56))
        };

        var judges = new List<Judge>
        {
            CreateJudge("judge-001", "judge.halyna", "Галина Петренко", "halyna.judge@kitchenbattle.com", now.AddDays(-100)),
            CreateJudge("judge-002", "judge.taras", "Тарас Левицький", "taras.judge@kitchenbattle.com", now.AddDays(-98)),
            CreateJudge("judge-003", "judge.lesia", "Леся Ковальчук", "lesia.judge@kitchenbattle.com", now.AddDays(-96)),
            CreateJudge("judge-004", "judge.artem", "Артем Соловей", "artem.judge@kitchenbattle.com", now.AddDays(-94)),
            CreateJudge("judge-005", "judge.oksana", "Оксана Яремчук", "oksana.judge@kitchenbattle.com", now.AddDays(-92)),
            CreateJudge("judge-006", "judge.petro", "Петро Василенко", "petro.judge@kitchenbattle.com", now.AddDays(-90)),
            CreateJudge("judge-007", "judge.diana", "Діана Ковтун", "diana.judge@kitchenbattle.com", now.AddDays(-88)),
            CreateJudge("judge-008", "judge.vladyslav", "Владислав Стефанюк", "vladyslav.judge@kitchenbattle.com", now.AddDays(-86))
        };

        var applicationUsers = new List<ApplicationUser>();
        applicationUsers.AddRange(admins.Select(a => CreateApplicationUser(a.Id, a.UserName ?? a.Email ?? a.FullName, a.CreatedAt)));
        applicationUsers.AddRange(chefs.Select(c => CreateApplicationUser(c.Id, c.UserName ?? c.Email ?? c.FullName, c.CreatedAt)));
        applicationUsers.AddRange(judges.Select(j => CreateApplicationUser(j.Id, j.UserName ?? j.Email ?? j.FullName, j.CreatedAt)));
        applicationUsers.AddRange(new[]
        {
            CreateApplicationUser("viewer-001", "viewer.anna", now.AddDays(-30)),
            CreateApplicationUser("viewer-002", "viewer.marko", now.AddDays(-25)),
            CreateApplicationUser("viewer-003", "viewer.yulia", now.AddDays(-18))
        });

        context.Admins.AddRange(admins);
        context.Chefs.AddRange(chefs);
        context.Judges.AddRange(judges);
        context.ApplicationUsers.AddRange(applicationUsers);
        await context.SaveChangesAsync();

        var battles = new List<Battle>
        {
            CreateBattle("Весняна битва десертів", "Ніжні десерти, авторські креми, сезонні ягоди та ресторанна подача.", StatusBattleEnum.Completed, CategoryEnum.Dessert, now.AddDays(-50), now.AddDays(-49), now.AddDays(-58), now.AddDays(-52)),
            CreateBattle("Український смак", "Традиційні українські страви в сучасному виконанні.", StatusBattleEnum.Completed, CategoryEnum.MainCourse, now.AddDays(-38), now.AddDays(-37), now.AddDays(-46), now.AddDays(-40)),
            CreateBattle("Морський сет", "Страви з риби та морепродуктів із легкою подачею.", StatusBattleEnum.Completed, CategoryEnum.Seafood, now.AddDays(-25), now.AddDays(-24), now.AddDays(-33), now.AddDays(-27)),
            CreateBattle("М’ясний гриль-челендж", "Стейки, реберця, авторські маринади й соуси до м’яса.", StatusBattleEnum.Closed, CategoryEnum.Meat, now.AddDays(-8), now.AddDays(-7), now.AddDays(-16), now.AddDays(-10)),
            CreateBattle("Сніданок чемпіона", "Поживні сніданки, яйця, тости, панкейки та корисні боули.", StatusBattleEnum.Closed, CategoryEnum.Breakfast, now.AddDays(-3), now.AddDays(-2), now.AddDays(-11), now.AddDays(-5)),
            CreateBattle("Вулична їжа світу", "Тако, бургери, роли, сендвічі та швидкі авторські закуски.", StatusBattleEnum.InProgress, CategoryEnum.Snack, now.AddDays(-1), now.AddDays(2), now.AddDays(-9), now.AddDays(-2)),
            CreateBattle("Вегетаріанський тиждень", "Овочі, крупи, бобові, соуси та повноцінні страви без м’яса.", StatusBattleEnum.InProgress, CategoryEnum.Vegetarian, now.AddDays(0), now.AddDays(4), now.AddDays(-8), now.AddDays(-1)),
            CreateBattle("Осінній суп-батл", "Гарячі супи, крем-супи, бульйони та сезонні овочі.", StatusBattleEnum.Pending, CategoryEnum.Soup, now.AddDays(8), now.AddDays(9), now.AddDays(1), now.AddDays(6)),
            CreateBattle("Фестиваль авторських соусів", "Соуси до м’яса, риби, овочів, пасти та закусок.", StatusBattleEnum.Pending, CategoryEnum.Sauce, now.AddDays(15), now.AddDays(16), now.AddDays(5), now.AddDays(13)),
            CreateBattle("Святкова фінальна битва", "Фінальні ресторанні страви з повною подачею та складною технікою.", StatusBattleEnum.Pending, CategoryEnum.HolidaySpecial, now.AddDays(28), now.AddDays(29), now.AddDays(18), now.AddDays(26))
        };

        context.Battles.AddRange(battles);
        await context.SaveChangesAsync();

        var battleChefs = new Dictionary<int, int[]>
        {
            [0] = new[] { 0, 1, 2, 3, 4, 5 },
            [1] = new[] { 6, 7, 8, 9, 10, 11 },
            [2] = new[] { 12, 13, 14, 15, 16, 17 },
            [3] = new[] { 0, 6, 8, 12, 14, 16 },
            [4] = new[] { 1, 3, 5, 7, 9, 11 },
            [5] = new[] { 2, 4, 6, 8, 10, 12 },
            [6] = new[] { 1, 5, 7, 11, 13, 17 },
            [7] = new[] { 0, 2, 9, 10, 15, 16 },
            [8] = new[] { 3, 4, 6, 13, 14, 17 },
            [9] = new[] { 5, 7, 8, 11, 12, 15 }
        };

        var battleJudges = new Dictionary<int, int[]>
        {
            [0] = new[] { 0, 1, 2 },
            [1] = new[] { 2, 3, 4 },
            [2] = new[] { 4, 5, 6 },
            [3] = new[] { 1, 3, 7 },
            [4] = new[] { 0, 5, 7 },
            [5] = new[] { 1, 4, 6 },
            [6] = new[] { 0, 2, 5 },
            [7] = new[] { 3, 6, 7 },
            [8] = new[] { 0, 4, 7 },
            [9] = new[] { 2, 5, 6 }
        };

        for (var battleIndex = 0; battleIndex < battles.Count; battleIndex++)
        {
            for (var i = 0; i < battleChefs[battleIndex].Length; i++)
            {
                context.BattleChefs.Add(new BattleChef
                {
                    BattleId = battles[battleIndex].Id,
                    ChefId = chefs[battleChefs[battleIndex][i]].Id,
                    IsApproved = battles[battleIndex].Status != StatusBattleEnum.Pending || i < 4
                });
            }

            foreach (var judgeIndex in battleJudges[battleIndex])
            {
                context.BattleJudges.Add(new BattleJudge
                {
                    BattleId = battles[battleIndex].Id,
                    JudgeId = judges[judgeIndex].Id
                });
            }
        }

        await context.SaveChangesAsync();

        var recipeSeeds = new List<RecipeSeed>
        {
            new(0, 0, "Медовий торт із солоною карамеллю", "Класичний медовик з карамельним кремом, горіховим крамблом і ягідним акцентом.", "борошно, мед, вершкове масло, яйця, сметанний крем, карамель, волоські горіхи, малина", 95, DifficultyEnum.Hard, StatusRecipeEnum.Published, CategoryEnum.Dessert, "https://images.unsplash.com/photo-1565958011703-44f9829ba187?auto=format&fit=crop&w=900&q=80"),
            new(0, 1, "Шоколадний фондан з ванільним морозивом", "Теплий шоколадний десерт із рідкою серцевиною та контрастом холодного морозива.", "темний шоколад, вершкове масло, яйця, цукор, борошно, ванільне морозиво, какао", 40, DifficultyEnum.Medium, StatusRecipeEnum.Published, CategoryEnum.Dessert, "https://images.unsplash.com/photo-1578985545062-69928b1d9587?auto=format&fit=crop&w=900&q=80"),
            new(0, 2, "Чізкейк Нью-Йорк з полуничним соусом", "Ніжний сирний десерт на пісочній основі з густим ягідним соусом.", "крем-сир, печиво, вершкове масло, яйця, вершки, цукор, полуниця, лимон", 120, DifficultyEnum.Hard, StatusRecipeEnum.Published, CategoryEnum.Dessert, "https://images.unsplash.com/photo-1533134242443-d4fd215305ad?auto=format&fit=crop&w=900&q=80"),
            new(0, 3, "Тірамісу з кавовим кремом", "Італійський десерт із маскарпоне, савоярді та насиченим кавовим ароматом.", "савоярді, маскарпоне, еспресо, какао, яйця, цукор, вершки", 65, DifficultyEnum.Medium, StatusRecipeEnum.Checked, CategoryEnum.Dessert, "https://images.unsplash.com/photo-1571877227200-a0d98ea607e9?auto=format&fit=crop&w=900&q=80"),
            new(1, 6, "Борщ із пампушками та часниковою олією", "Насичений український борщ із яловичиною, квасолею, сметаною та домашніми пампушками.", "буряк, капуста, картопля, морква, цибуля, яловичина, квасоля, томати, часник, сметана", 150, DifficultyEnum.Hard, StatusRecipeEnum.Published, CategoryEnum.Soup, "https://images.unsplash.com/photo-1547592180-85f173990554?auto=format&fit=crop&w=900&q=80"),
            new(1, 7, "Вареники з картоплею та грибною підливою", "Домашні вареники з м’яким тістом, картопляною начинкою та ароматними грибами.", "борошно, яйце, картопля, цибуля, печериці, вершки, масло, зелень", 105, DifficultyEnum.Medium, StatusRecipeEnum.Published, CategoryEnum.MainCourse, "https://images.unsplash.com/photo-1551183053-bf91a1d81141?auto=format&fit=crop&w=900&q=80"),
            new(1, 8, "Котлета по-київськи з картопляним пюре", "Хрустка паніровка, соковите куряче філе та вершкове масло із зеленню усередині.", "куряче філе, вершкове масло, кріп, яйця, сухарі, картопля, молоко", 90, DifficultyEnum.Hard, StatusRecipeEnum.Published, CategoryEnum.MainCourse, "https://images.unsplash.com/photo-1604908176997-125f25cc6f3d?auto=format&fit=crop&w=900&q=80"),
            new(1, 9, "Голубці у томатно-сметанному соусі", "Класичні голубці з рисом і м’ясом, запечені в густому соусі.", "капуста, рис, фарш, морква, цибуля, томатний соус, сметана, спеції", 135, DifficultyEnum.Hard, StatusRecipeEnum.Published, CategoryEnum.MainCourse, "https://images.unsplash.com/photo-1604909052743-94e838986d24?auto=format&fit=crop&w=900&q=80"),
            new(2, 12, "Філе лосося з лимонним маслом", "Ніжний лосось зі скоринкою, легким соусом і гарніром зі спаржі.", "лосось, лимон, вершкове масло, спаржа, часник, оливкова олія, кріп", 45, DifficultyEnum.Medium, StatusRecipeEnum.Published, CategoryEnum.Seafood, "https://images.unsplash.com/photo-1467003909585-2f8a72700288?auto=format&fit=crop&w=900&q=80"),
            new(2, 13, "Паста з морепродуктами у вершковому соусі", "Паста з креветками, мідіями, часником, пармезаном і білим соусом.", "паста, креветки, мідії, вершки, пармезан, часник, петрушка, лимон", 50, DifficultyEnum.Medium, StatusRecipeEnum.Published, CategoryEnum.Seafood, "https://images.unsplash.com/photo-1563379926898-05f4575a45d8?auto=format&fit=crop&w=900&q=80"),
            new(2, 14, "Креветки темпура з манговим соусом", "Хрусткі креветки в легкому клярі з кисло-солодким манговим соусом.", "креветки, борошно, крохмаль, яйце, манго, лайм, чилі, кінза", 38, DifficultyEnum.Medium, StatusRecipeEnum.Published, CategoryEnum.Seafood, "https://images.unsplash.com/photo-1562967916-eb82221dfb92?auto=format&fit=crop&w=900&q=80"),
            new(2, 15, "Салат із тунцем, яйцем і зеленою квасолею", "Легка страва з тунцем, овочами, яйцем і гірчичною заправкою.", "тунець, яйця, зелена квасоля, томати, салат, оливки, гірчиця, оливкова олія", 35, DifficultyEnum.Easy, StatusRecipeEnum.Checked, CategoryEnum.Salad, "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=900&q=80"),
            new(3, 0, "Рібай-стейк із розмариновим маслом", "Соковитий стейк середнього просмаження з ароматним маслом і печеними овочами.", "яловичий стейк, розмарин, вершкове масло, часник, перець, сіль, овочі", 55, DifficultyEnum.Hard, StatusRecipeEnum.Published, CategoryEnum.Meat, "https://images.unsplash.com/photo-1558030006-450675393462?auto=format&fit=crop&w=900&q=80"),
            new(3, 6, "Свинячі реберця BBQ", "Повільно запечені реберця з глазур’ю барбекю та салатом коул-слоу.", "свинячі ребра, соус BBQ, паприка, мед, капуста, морква, йогурт", 180, DifficultyEnum.Hard, StatusRecipeEnum.Published, CategoryEnum.Meat, "https://images.unsplash.com/photo-1529193591184-b1d58069ecdd?auto=format&fit=crop&w=900&q=80"),
            new(3, 8, "Курячі шашлички з йогуртовим маринадом", "Ніжне куряче філе на шпажках із пряним маринадом і зеленню.", "куряче філе, йогурт, лимон, часник, паприка, коріандр, зелень", 65, DifficultyEnum.Medium, StatusRecipeEnum.Published, CategoryEnum.Meat, "https://images.unsplash.com/photo-1600891964599-f61ba0e24092?auto=format&fit=crop&w=900&q=80"),
            new(3, 12, "Бургер із яловичиною та карамелізованою цибулею", "Соковита котлета, сир чедер, соус, мариновані огірки та м’яка булочка.", "булочка, яловичина, чедер, цибуля, огірки, томати, салат, соус", 45, DifficultyEnum.Medium, StatusRecipeEnum.Published, CategoryEnum.Snack, "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=900&q=80"),
            new(4, 1, "Панкейки з бананом і кленовим сиропом", "Повітряні панкейки з бананом, ягодами та солодким сиропом.", "борошно, молоко, яйця, банан, розпушувач, кленовий сироп, ягоди", 30, DifficultyEnum.Easy, StatusRecipeEnum.Published, CategoryEnum.Breakfast, "https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?auto=format&fit=crop&w=900&q=80"),
            new(4, 3, "Авокадо-тост із яйцем пашот", "Хрусткий тост з авокадо, яйцем пашот, зеленню та насінням.", "хліб, авокадо, яйця, лимон, мікрогрін, кунжут, оливкова олія", 25, DifficultyEnum.Medium, StatusRecipeEnum.Published, CategoryEnum.Breakfast, "https://images.unsplash.com/photo-1525351484163-7529414344d8?auto=format&fit=crop&w=900&q=80"),
            new(4, 5, "Сирники з ягідним соусом", "Рум’яні сирники з кисломолочного сиру, сметаною та ягідною поливкою.", "кисломолочний сир, яйця, борошно, цукор, ваніль, сметана, ягоди", 45, DifficultyEnum.Easy, StatusRecipeEnum.Published, CategoryEnum.Breakfast, "https://images.unsplash.com/photo-1495214783159-3503fd1b572d?auto=format&fit=crop&w=900&q=80"),
            new(4, 7, "Омлет із томатами та фетою", "Легкий омлет із зеленню, томатами, сиром фета та салатним міксом.", "яйця, молоко, томати, фета, шпинат, зелень, масло", 20, DifficultyEnum.Easy, StatusRecipeEnum.Draft, CategoryEnum.Breakfast, "https://images.unsplash.com/photo-1482049016688-2d3e1b311543?auto=format&fit=crop&w=900&q=80"),
            new(5, 2, "Тако з куркою та сальсою", "Кукурудзяні тортильї з куркою, овочами, сальсою та лаймом.", "тортильї, курка, томати, цибуля, лайм, кукурудза, авокадо, кінза", 40, DifficultyEnum.Medium, StatusRecipeEnum.Published, CategoryEnum.Snack, "https://images.unsplash.com/photo-1565299585323-38d6b0865b47?auto=format&fit=crop&w=900&q=80"),
            new(5, 4, "Хот-дог із карамелізованою цибулею", "Авторський хот-дог із соусом, сиром, маринованими огірками та цибулею.", "булочка, сосиска, цибуля, сир, огірки, гірчиця, кетчуп, зелень", 25, DifficultyEnum.Easy, StatusRecipeEnum.Published, CategoryEnum.Snack, "https://images.unsplash.com/photo-1612392062631-94dd858cba88?auto=format&fit=crop&w=900&q=80"),
            new(5, 6, "Сендвіч із ростбіфом", "Ситний сендвіч із ростбіфом, соусом, салатом і маринованою цибулею.", "хліб, ростбіф, салат, цибуля, гірчичний соус, огірки, сир", 35, DifficultyEnum.Medium, StatusRecipeEnum.Checked, CategoryEnum.Snack, "https://images.unsplash.com/photo-1528735602780-2552fd46c7af?auto=format&fit=crop&w=900&q=80"),
            new(5, 8, "Домашня піца з прошуто", "Тонке тісто, томатний соус, моцарела, прошуто та рукола.", "тісто, томатний соус, моцарела, прошуто, рукола, оливкова олія", 70, DifficultyEnum.Medium, StatusRecipeEnum.Published, CategoryEnum.Snack, "https://images.unsplash.com/photo-1513104890138-7c749659a591?auto=format&fit=crop&w=900&q=80"),
            new(6, 1, "Овочеве рагу з нутом", "Поживне овочеве рагу з нутом, томатами та ароматними спеціями.", "нут, баклажан, кабачок, томати, перець, цибуля, часник, паприка", 65, DifficultyEnum.Easy, StatusRecipeEnum.Published, CategoryEnum.Vegetarian, "https://images.unsplash.com/photo-1476124369491-e7addf5db371?auto=format&fit=crop&w=900&q=80"),
            new(6, 5, "Грибне різото з пармезаном", "Кремове різото з грибами, білим вином, пармезаном і зеленню.", "рис арборіо, гриби, цибуля, пармезан, бульйон, вершкове масло, петрушка", 55, DifficultyEnum.Medium, StatusRecipeEnum.Published, CategoryEnum.Vegetarian, "https://images.unsplash.com/photo-1476124369491-e7addf5db371?auto=format&fit=crop&w=900&q=80"),
            new(6, 7, "Салат із кіноа та печеним гарбузом", "Теплий салат із кіноа, гарбузом, сиром, зеленню та горіхами.", "кіноа, гарбуз, рукола, фета, волоські горіхи, мед, гірчиця", 45, DifficultyEnum.Easy, StatusRecipeEnum.Published, CategoryEnum.Salad, "https://images.unsplash.com/photo-1540420773420-3366772f4999?auto=format&fit=crop&w=900&q=80"),
            new(6, 11, "Веганський боул із тофу", "Яскравий боул з тофу, рисом, овочами, кунжутом і соєвим соусом.", "тофу, рис, морква, огірок, едамаме, соєвий соус, кунжут, авокадо", 40, DifficultyEnum.Easy, StatusRecipeEnum.Published, CategoryEnum.Vegan, "https://images.unsplash.com/photo-1512621776951-a57141f2eefd?auto=format&fit=crop&w=900&q=80"),
            new(7, 0, "Крем-суп із гарбуза", "Ніжний гарбузовий крем-суп із вершками, насінням і пряними нотами.", "гарбуз, морква, цибуля, вершки, бульйон, гарбузове насіння, імбир", 50, DifficultyEnum.Easy, StatusRecipeEnum.Draft, CategoryEnum.Soup, "https://images.unsplash.com/photo-1547592166-23ac45744acd?auto=format&fit=crop&w=900&q=80"),
            new(7, 2, "Рамен із куркою", "Азійський суп з локшиною, куркою, яйцем, грибами та зеленню.", "курка, локшина, яйце, гриби, соєвий соус, бульйон, зелена цибуля", 90, DifficultyEnum.Hard, StatusRecipeEnum.Checked, CategoryEnum.Soup, "https://images.unsplash.com/photo-1569718212165-3a8278d5f624?auto=format&fit=crop&w=900&q=80"),
            new(7, 9, "Грибний крем-суп із сухариками", "Ароматний крем-суп із печериць із вершками, грінками та зеленню.", "печериці, вершки, цибуля, картопля, бульйон, хліб, зелень", 45, DifficultyEnum.Easy, StatusRecipeEnum.Published, CategoryEnum.Soup, "https://images.unsplash.com/photo-1547592180-85f173990554?auto=format&fit=crop&w=900&q=80"),
            new(7, 10, "Томатний суп із базиліком", "Легкий томатний суп з базиліком, часником і хрустким тостом.", "томати, базилік, часник, цибуля, оливкова олія, бульйон, хліб", 35, DifficultyEnum.Easy, StatusRecipeEnum.Rejected, CategoryEnum.Soup, "https://images.unsplash.com/photo-1569411923288-4c9a9195d570?auto=format&fit=crop&w=900&q=80"),
            new(8, 3, "Вершково-грибний соус", "Густий соус до пасти, м’яса або картоплі з виразним грибним смаком.", "гриби, вершки, цибуля, часник, пармезан, вершкове масло, перець", 30, DifficultyEnum.Easy, StatusRecipeEnum.Checked, CategoryEnum.Sauce, "https://images.unsplash.com/photo-1476224203421-9ac39bcb3327?auto=format&fit=crop&w=900&q=80"),
            new(8, 4, "Гострий томатний соус", "Насичений томатний соус із чилі, часником і травами.", "томати, чилі, часник, базилік, оливкова олія, орегано, сіль", 40, DifficultyEnum.Easy, StatusRecipeEnum.Draft, CategoryEnum.Sauce, "https://images.unsplash.com/photo-1476224203421-9ac39bcb3327?auto=format&fit=crop&w=900&q=80"),
            new(8, 6, "Медово-гірчичний соус", "Баланс солодкості, кислоти й пікантності для м’яса та салатів.", "мед, діжонська гірчиця, лимон, оливкова олія, перець, сіль", 15, DifficultyEnum.Easy, StatusRecipeEnum.Published, CategoryEnum.Sauce, "https://images.unsplash.com/photo-1504674900247-0877df9cc836?auto=format&fit=crop&w=900&q=80"),
            new(8, 13, "Соус песто з волоським горіхом", "Зелений соус із базиліком, сиром, горіхами та оливковою олією.", "базилік, волоські горіхи, пармезан, часник, оливкова олія, лимон", 20, DifficultyEnum.Easy, StatusRecipeEnum.Published, CategoryEnum.Sauce, "https://images.unsplash.com/photo-1476224203421-9ac39bcb3327?auto=format&fit=crop&w=900&q=80"),
            new(9, 5, "Качина грудка з ягідним соусом", "Ресторанна страва з качиною грудкою, ягідним соусом і овочевим гарніром.", "качина грудка, ягоди, червоне вино, морква, пастернак, масло, спеції", 85, DifficultyEnum.Hard, StatusRecipeEnum.Checked, CategoryEnum.HolidaySpecial, "https://images.unsplash.com/photo-1504674900247-0877df9cc836?auto=format&fit=crop&w=900&q=80"),
            new(9, 7, "Запечена індичка з травами", "Святкова індичка з ароматними травами, цитрусами та соусом.", "індичка, розмарин, апельсин, часник, вершкове масло, мед, спеції", 210, DifficultyEnum.Hard, StatusRecipeEnum.Draft, CategoryEnum.HolidaySpecial, "https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&w=900&q=80"),
            new(9, 8, "Святковий салат із гранатом", "Яскравий салат із куркою, сиром, горіхами, гранатом і легкою заправкою.", "курка, гранат, сир, горіхи, салат, йогурт, гірчиця, зелень", 45, DifficultyEnum.Medium, StatusRecipeEnum.Published, CategoryEnum.HolidaySpecial, "https://images.unsplash.com/photo-1540189549336-e6e99c3679fe?auto=format&fit=crop&w=900&q=80"),
            new(9, 11, "Шоколадний рулет із вишнею", "Святковий десерт із шоколадним бісквітом, кремом і вишневою начинкою.", "какао, яйця, борошно, вершки, вишня, цукор, шоколад", 75, DifficultyEnum.Medium, StatusRecipeEnum.Checked, CategoryEnum.HolidaySpecial, "https://images.unsplash.com/photo-1488477181946-6428a0291777?auto=format&fit=crop&w=900&q=80")
        };

        var recipes = new List<(Recipe Recipe, int BattleIndex)>();

        foreach (var seed in recipeSeeds)
        {
            var chef = chefs[seed.ChefIndex];
            var battle = battles[seed.BattleIndex];
            var recipe = new Recipe
            {
                ChefId = chef.Id,
                ImageUrl = seed.ImageUrl,
                Title = seed.Title,
                Description = seed.Description,
                Ingredients = seed.Ingredients,
                CookingTime = seed.CookingTime,
                Difficulty = seed.Difficulty,
                ChefName = chef.FullName,
                Status = seed.Status,
                AverageScore = 0,
                Category = seed.Category,
                CreatedAt = battle.RegistrationEnd.AddDays(-1).AddHours(seed.ChefIndex % 8)
            };

            context.Recipes.Add(recipe);
            SetShadowBattleId(context, recipe, battle.Id);
            recipes.Add((recipe, seed.BattleIndex));
        }

        await context.SaveChangesAsync();

        var random = new Random(27);

        foreach (var item in recipes)
        {
            var recipe = item.Recipe;
            var battle = battles[item.BattleIndex];

            if (recipe.Status != StatusRecipeEnum.Published)
                continue;

            if (battle.Status == StatusBattleEnum.Pending)
                continue;

            var judgeIndexes = battleJudges[item.BattleIndex];
            var scoreCount = battle.Status == StatusBattleEnum.InProgress ? 2 : judgeIndexes.Length;

            for (var i = 0; i < scoreCount; i++)
            {
                var judge = judges[judgeIndexes[i]];
                var totalShift = random.Next(-2, 3);
                var taste = Math.Clamp(7 + random.Next(0, 4) + totalShift, 5, 10);
                var presentation = Math.Clamp(6 + random.Next(0, 5) + totalShift, 5, 10);
                var creativity = Math.Clamp(6 + random.Next(0, 5) + totalShift, 5, 10);

                context.Scores.Add(new Score
                {
                    RecipeId = recipe.Id,
                    JudgeId = judge.Id,
                    Taste = taste,
                    Presentation = presentation,
                    Creativity = creativity,
                    Comments = CreateComment(taste + presentation + creativity, recipe.Title)
                });
            }
        }

        await context.SaveChangesAsync();

        foreach (var recipe in recipes.Select(r => r.Recipe))
        {
            var scores = await context.Scores
                .Where(s => s.RecipeId == recipe.Id)
                .ToListAsync();

            recipe.AverageScore = scores.Any()
                ? Math.Round(scores.Average(s => s.TotalScore), 2)
                : 0;
        }

        foreach (var battleIndex in new[] { 0, 1, 2, 3, 4 })
        {
            var battleRecipes = recipes
                .Where(r => r.BattleIndex == battleIndex && r.Recipe.Status == StatusRecipeEnum.Published && r.Recipe.AverageScore > 0)
                .Select(r => r.Recipe)
                .OrderByDescending(r => r.AverageScore)
                .ToList();

            if (battleRecipes.Any())
                battles[battleIndex].WinnerId = battleRecipes.First().ChefId;
        }

        await context.SaveChangesAsync();
    }

    private static Admin CreateAdmin(string id, string userName, string fullName, string email, DateTime createdAt)
    {
        return new Admin
        {
            Id = id,
            UserName = userName,
            NormalizedUserName = userName.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            EmailConfirmed = true,
            FullName = fullName,
            CreatedAt = createdAt
        };
    }

    private static Chef CreateChef(string id, string userName, string fullName, string email, DateTime createdAt)
    {
        return new Chef
        {
            Id = id,
            UserName = userName,
            NormalizedUserName = userName.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            EmailConfirmed = true,
            FullName = fullName,
            CreatedAt = createdAt
        };
    }

    private static Judge CreateJudge(string id, string userName, string fullName, string email, DateTime createdAt)
    {
        return new Judge
        {
            Id = id,
            UserName = userName,
            NormalizedUserName = userName.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            EmailConfirmed = true,
            FullName = fullName,
            CreatedAt = createdAt
        };
    }

    private static ApplicationUser CreateApplicationUser(string id, string userName, DateTime createdAt)
    {
        return new ApplicationUser
        {
            Id = id,
            UserName = userName,
            CreatedAt = createdAt
        };
    }

    private static Battle CreateBattle(string battleName, string description, StatusBattleEnum status, CategoryEnum category, DateTime startedAt, DateTime? endedAt, DateTime registrationStart, DateTime registrationEnd)
    {
        return new Battle
        {
            WinnerId = string.Empty,
            BattleName = battleName,
            Description = description,
            Status = status,
            StartedAt = startedAt,
            EndedAt = endedAt,
            RegistrationStart = registrationStart,
            RegistrationEnd = registrationEnd,
            Category = category
        };
    }

    private static string CreateComment(int totalScore, string recipeTitle)
    {
        if (totalScore >= 28)
            return $"{recipeTitle}: сильна ресторанна робота, добре збалансований смак і впевнена подача.";

        if (totalScore >= 24)
            return $"{recipeTitle}: якісне виконання, страва виглядає завершеною, але є простір для точнішого акценту.";

        if (totalScore >= 20)
            return $"{recipeTitle}: цікава ідея, проте потрібно допрацювати баланс смаку або презентацію.";

        return $"{recipeTitle}: задум зрозумілий, але техніка й подача потребують суттєвого покращення.";
    }

    private static void SetShadowBattleId(ApplicationDbContext context, Recipe recipe, int battleId)
    {
        var entry = context.Entry(recipe);
        if (entry.Metadata.FindProperty("BattleId") != null)
            entry.Property<int?>("BattleId").CurrentValue = battleId;
    }

    private sealed record RecipeSeed(
        int BattleIndex,
        int ChefIndex,
        string Title,
        string Description,
        string Ingredients,
        int CookingTime,
        DifficultyEnum Difficulty,
        StatusRecipeEnum Status,
        CategoryEnum Category,
        string ImageUrl);
}
