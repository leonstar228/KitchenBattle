using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using KitchenBattle.Models;
using KitchenBattle.Data; // ваш DbContext

namespace KitchenBattle.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Головна сторінка може показувати останні батли
            var battles = _context.Battles
                .OrderByDescending(b => b.StartedAt)
                .Take(5)
                .ToList();

            return View(battles);
        }

        public IActionResult Battles()
        {
            // Всі батли
            var battles = _context.Battles.ToList();
            return View(battles);
        }

        public IActionResult Recipes()
        {
            // Всі рецепти
            var recipes = _context.Recipes.ToList();
            return View(recipes);
        }

        public IActionResult Leaderboard()
        {
            // Лідерборд поточного батлу
            var currentBattle = _context.Battles
                .OrderByDescending(b => b.StartedAt)
                .FirstOrDefault();

            return View(currentBattle);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}

