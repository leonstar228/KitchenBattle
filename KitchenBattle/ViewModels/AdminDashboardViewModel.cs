namespace KitchenBattle.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalChefs { get; set; }
        public int TotalJudges { get; set; }

        public int TotalRecipes { get; set; }
        public int PublishedRecipes { get; set; }
        public int PendingRecipes { get; set; }
        public int RejectedRecipes { get; set; }

        public int TotalBattles { get; set; }
        public int ActiveBattles { get; set; }
        public int FinishedBattles { get; set; }

        public double GlobalAverageScore { get; set; }
        public string MostPopularCategory { get; set; }
    }
}