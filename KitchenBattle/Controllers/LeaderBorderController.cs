using Microsoft.AspNetCore.Mvc;
using KitchenBattle.Services;

namespace KitchenBattle.Controllers
{
    public class LeaderBoardController : Controller
    {
        private readonly LeaderBoardService _leaderBoardService;

        public LeaderBoardController(LeaderBoardService leaderBoardService)
        {
            _leaderBoardService = leaderBoardService;
        }

        public async Task<IActionResult> Index()
        {
            var leaderboard = await _leaderBoardService.GetTopLeaderboard();
            return View(leaderboard);
        }
    }
}