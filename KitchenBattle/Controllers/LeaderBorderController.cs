using Microsoft.AspNetCore.Mvc;
using KitchenBattle.Services;


namespace KitchenBattle.Controllers
{
    public class LeaderBoardController : Controller
    {
        private readonly LeaderBoardService _leaderBoardService;
        private readonly RedisService _redisService;

        public LeaderBoardController(LeaderBoardService leaderBoardService, RedisService redisService)
        {
            _leaderBoardService = leaderBoardService;
            _redisService = redisService;
        }

        public async Task<IActionResult> Index()
        {
            var leaderboard = await _redisService.GetLeaderBoardCach();
            return View(leaderboard);
        }
    }
}