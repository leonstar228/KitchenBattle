namespace KitchenBattle.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public int ChefId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public int CookingTime { get; set; }
        public DifficultyEnum Difficulty { get; set; } = DifficultyEnum.Easy;
        public string ChefName { get; set; } = string.Empty;
        public StatusRecipeEnum Status { get; set; } = StatusRecipeEnum.Draft;
        public double AverageScore { get; set; }
        public ICollection<Score> Scores { get; set; } = new List<Score>();
        public CategoryEnum Category { get; internal set; }
    }
}
