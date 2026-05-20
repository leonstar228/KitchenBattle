# 👑 RecipeRoyale — Кулінарний батл

> *«Кожен вважає себе хорошим кухарем — але чи витримає твій рецепт суддівський вердикт?»*

RecipeRoyale — це платформа кулінарних змагань, де шефи публікують рецепти, судді виставляють оцінки в реальному часі, а система автоматично формує лідерборд переможців. Усе чесно: категорії, критерії, один суддя — одна оцінка.

---

## 🎭 Ролі

| Роль | Що робить |
|------|-----------|
| 👨‍🍳 **Chef** | Публікує рецепти на поточний батл |
| ⚖️ **Judge** | Оцінює рецепти за 3 критеріями від 1 до 10 |
| 🛡️ **Admin** | Керує батлами, категоріями та учасниками |

---

## 🗃️ Моделі даних

### Recipe
```
Title | Description | Ingredients | CookingTime | Difficulty
Category | ChefId | ChefName | Status | AverageScore
```

### Score
```
RecipeId | JudgeId | JudgeName | Taste | Presentation
Creativity | TotalScore | Comment
```

---

## ⚖️ Система оцінювання

```
AverageScore = ( Taste + Presentation + Creativity ) / 3
```

- 📊 Кожен критерій — від **1 до 10**
- 🔄 `AverageScore` перераховується **автоматично** після кожної нової оцінки
- 🔒 Один суддя може поставити **лише одну оцінку** на рецепт — без накруток

---

## 🍽️ Категорії батлів

| | |
|---|---|
| 🍰 Десерти | 🍲 Супи |
| 🍖 Основні страви | 🥗 Закуски |

---

## ✨ Ключові фічі

### 🏆 Лідерборд
- **Топ рецептів** поточного батлу — оновлюється в реальному часі
- **Топ шефів за всі часи** — рейтинг найкращих кухарів платформи

### 📊 Дашборд
- Кількість учасників поточного батлу
- Середній бал по категоріях

### ⚡ Redis кешування
Для максимальної швидкості відповідей кешуються:
- Лідерборд поточного батлу
- Список рецептів батлу
- Статистика дашборду

---

## 🎨 Frontend Style Guide

### Кольорова палітра

| Назва | HEX | Використання |
|---|---|---|
| Primary | `#4A6741` | Основний бренд, картки, CTA |
| Secondary | `#C47B2B` | Кнопки, акценти, ціни |
| Background | `#F5F1EB` | Фон сторінок |
| Dark | `#2C2C2A` | Текст, navbar, footer |
| Green 400 | `#6B8F5E` | Hover-стани, secondary cards |
| Gold 200 | `#E8D5A3` | Бейджі, бордери |
| Neutral 600 | `#8B7355` | Підтекст, іконки |
| Accent Red | `#D4483B` | Знижки, warning, hot labels |
| White | `#FFFFFF` | Картки, секції |

---

## 🔁 Як це працює

```
1. Chef публікує рецепт на поточний батл
       ↓
2. Judge оцінює: Taste / Presentation / Creativity (1–10)
       ↓
3. Система автоматично перераховує AverageScore
       ↓
4. Redis-кеш інвалідується → лідерборд оновлюється миттєво
```

---

## 🗺️ Карта проекту

### 1) Home

```
Home/Index.cshtml
│
├── → Battles/Index.cshtml
│       Перехід: "Переглянути батли"
│
├── → Recipes/Index.cshtml
│       Перехід: "Переглянути рецепти"
│
├── → Leaderboard/CurrentBattle.cshtml
│       Перехід: "Лідерборд"
│
└── → Home/Privacy.cshtml
        Перехід: "Політика конфіденційності"
```

### 2) Battles

```
Battles/Index.cshtml
│
├── → Battles/Details.cshtml
│       Перехід: "Деталі батлу"
│
├── → Battles/Create.cshtml
│       Перехід: "Створити battle"
│       Доступ: Admin
│
└── → Home/Index.cshtml
        Перехід: "На головну"


Battles/Details.cshtml
│
├── → Battles/Edit.cshtml
│       Перехід: "Редагувати battle"
│       Доступ: Admin
│
├── → Battles/Delete.cshtml
│       Перехід: "Видалити battle"
│       Доступ: Admin
│
├── → Recipes/Details.cshtml
│       Перехід: "Переглянути рецепт"
│
├── → Recipes/Create.cshtml
│       Перехід: "Додати рецепт до battle"
│       Доступ: Chef
│
├── → Scores/Create.cshtml
│       Перехід: "Оцінити рецепт"
│       Доступ: Judge
│
└── → Battles/Index.cshtml
        Перехід: "Назад до списку батлів"


Battles/Create.cshtml
│
└── → Battles/Index.cshtml
        Після створення battle


Battles/Edit.cshtml
│
└── → Battles/Details.cshtml
        Після редагування battle


Battles/Delete.cshtml
│
└── → Battles/Index.cshtml
        Після видалення battle
```

### 3) Recipes

```
Recipes/Index.cshtml
│
├── → Recipes/Details.cshtml
│       Перехід: "Деталі рецепта"
│
├── → Recipes/Create.cshtml
│       Перехід: "Створити рецепт"
│       Доступ: Chef
│
├── → Recipes/MyRecipes.cshtml
│       Перехід: "Мої рецепти"
│       Доступ: Chef
│
└── → Home/Index.cshtml
        Перехід: "На головну"


Recipes/MyRecipes.cshtml
│
├── → Recipes/Create.cshtml
│       Перехід: "Додати рецепт"
│
├── → Recipes/Edit.cshtml
│       Перехід: "Редагувати"
│
├── → Recipes/Delete.cshtml
│       Перехід: "Видалити"
│
├── → Recipes/Details.cshtml
│       Перехід: "Деталі"
│
└── → Recipes/Index.cshtml
        Перехід: "Всі рецепти"


Recipes/Details.cshtml
│
├── → Scores/Create.cshtml
│       Перехід: "Поставити оцінку"
│       Доступ: Judge
│
├── → Recipes/Edit.cshtml
│       Перехід: "Редагувати рецепт"
│       Доступ: Chef/Admin
│
├── → Recipes/Delete.cshtml
│       Перехід: "Видалити рецепт"
│       Доступ: Chef/Admin
│
├── → Recipes/RecipeScores.cshtml
│       Перехід: "Переглянути оцінки"
│
└── → Recipes/Index.cshtml
        Перехід: "Назад до рецептів"


Recipes/Create.cshtml
│
└── → Recipes/MyRecipes.cshtml
        Після створення рецепта


Recipes/Edit.cshtml
│
└── → Recipes/Details.cshtml
        Після редагування рецепта


Recipes/Delete.cshtml
│
└── → Recipes/Index.cshtml
        Після видалення рецепта


Recipes/RecipeScores.cshtml
│
├── → Scores/Edit.cshtml
│       Перехід: "Редагувати оцінку"
│       Доступ: Judge
│
└── → Recipes/Details.cshtml
        Перехід: "Назад до рецепта"
```

### 4) Scores

```
Scores/Create.cshtml
│
└── → Recipes/Details.cshtml
        Після додавання оцінки


Scores/Edit.cshtml
│
└── → Recipes/RecipeScores.cshtml
        Після редагування оцінки


Scores/Delete.cshtml
│
└── → Recipes/RecipeScores.cshtml
        Після видалення оцінки
```

### 5) Admin

```
Admin/Dashboard.cshtml
│
├── → Admin/RecipesForReview.cshtml
│       Перехід: "Рецепти на перевірку"
│
├── → Admin/Users.cshtml
│       Перехід: "Користувачі"
│
├── → Admin/Statistics.cshtml
│       Перехід: "Статистика"
│
├── → Battles/Create.cshtml
│       Перехід: "Створити battle"
│
└── → Home/Index.cshtml
        Перехід: "На головну"


Admin/RecipesForReview.cshtml
│
├── → Recipes/Details.cshtml
│       Перехід: "Переглянути рецепт"
│
├── → Recipes/Approve
│       Дія: "Схвалити рецепт"
│
├── → Recipes/Reject
│       Дія: "Відхилити рецепт"
│
└── → Admin/Dashboard.cshtml
        Перехід: "Назад до панелі адміністратора"


Admin/Users.cshtml
│
├── → Admin/CreateChef
│       Перехід: "Створити Chef"
│
├── → Admin/CreateJudge
│       Перехід: "Створити Judge"
│
├── → Admin/DeleteUser
│       Дія: "Видалити користувача"
│
└── → Admin/Dashboard.cshtml
        Перехід: "Назад до Dashboard"


Admin/Statistics.cshtml
│
├── → Leaderboard/BestRecipes.cshtml
│       Перехід: "Топ рецептів"
│
├── → Leaderboard/BestChefs.cshtml
│       Перехід: "Топ Chef"
│
└── → Admin/Dashboard.cshtml
        Перехід: "Назад до Dashboard"
```

### 6) Leaderboard

```
Leaderboard/CurrentBattle.cshtml
│
├── → Recipes/Details.cshtml
│       Перехід: "Переглянути рецепт"
│
├── → Battles/Details.cshtml
│       Перехід: "Переглянути battle"
│
└── → Home/Index.cshtml
        Перехід: "На головну"


Leaderboard/BestRecipes.cshtml
│
├── → Recipes/Details.cshtml
│       Перехід: "Переглянути рецепт"
│
└── → Leaderboard/AllTimeTop.cshtml
        Перехід: "Загальний рейтинг"


Leaderboard/BestChefs.cshtml
│
├── → Recipes/Index.cshtml
│       Перехід: "Рецепти Chef"
│
└── → Leaderboard/AllTimeTop.cshtml
        Перехід: "Загальний рейтинг"


Leaderboard/AllTimeTop.cshtml
│
├── → Leaderboard/BestRecipes.cshtml
│       Перехід: "Кращі рецепти"
│
├── → Leaderboard/BestChefs.cshtml
│       Перехід: "Кращі Chef"
│
└── → Home/Index.cshtml
        Перехід: "На головну"
```

### 7) Shared Layout

```
Shared/_Layout.cshtml
│
├── → Home/Index.cshtml
│       Меню: "Головна"
│
├── → Battles/Index.cshtml
│       Меню: "Battles"
│
├── → Recipes/Index.cshtml
│       Меню: "Recipes"
│
├── → Leaderboard/CurrentBattle.cshtml
│       Меню: "Leaderboard"
│
├── → Recipes/MyRecipes.cshtml
│       Меню: "Мої рецепти"
│       Доступ: Chef
│
├── → Admin/Dashboard.cshtml
│       Меню: "Admin Panel"
│       Доступ: Admin
│
├── → Keycloak Login
│       Меню: "Увійти"
│
└── → Keycloak Logout
        Меню: "Вийти"
```

---

## 🛠️ Технології

- **Backend:** ASP.NET Core
- **Auth / Realm:** Keycloak (`kitchenbattle`)
- **Cache:** Redis
- **Database:** PostgreSQL / SQL Server

---

## 🚀 Запуск проекту

```bash
# 1. Клонуємо репозиторій
git clone https://github.com/your-team/recipe-royale.git
cd recipe-royale

# 2. Запускаємо залежності (Redis, БД, Keycloak)
docker-compose up -d

# 3. Запускаємо застосунок
dotnet run
```

> Переконайтесь, що Keycloak realm `kitchenbattle` імпортовано перед першим запуском.

---

## 👥 Команда

Командний навчальний проект. Contributions welcome — відкривайте PR та Issues!

---

*Made with 🔥 and a lot of taste.*
