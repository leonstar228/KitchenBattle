using Microsoft.EntityFrameworkCore;
using KitchenBattle.Models;

namespace KitchenBattle.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            _options = options;
        }

        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<Chef> Chefs => Set<Chef>();
        public DbSet<Judge> Judges => Set<Judge>();
        public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
        public DbSet<BattleChef> BattleChefs => Set<BattleChef>();
        public DbSet<BattleJudge> BattleJudges => Set<BattleJudge>();
        public DbSet<Recipe> Recipes => Set<Recipe>();
        public DbSet<Battle> Battles => Set<Battle>();
        public DbSet<Score> Scores => Set<Score>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recipe>()
                 .HasOne(r => r.Chef)
     .WithMany(u => u.Recipes)
     .HasForeignKey(r => r.ChefId)
     .OnDelete(DeleteBehavior.Restrict);

            // BattleChef
            modelBuilder.Entity<BattleChef>()
                .HasKey(bc => new { bc.BattleId, bc.ChefId });

            modelBuilder.Entity<BattleChef>()
                .HasOne(bc => bc.Battle)
                .WithMany(b => b.BattleChefs)
                .HasForeignKey(bc => bc.BattleId);

            modelBuilder.Entity<BattleChef>()
                .HasOne(bc => bc.Chef)
                .WithMany(c => c.BattleChefs)
                .HasForeignKey(bc => bc.ChefId);

            // BattleJudge
            modelBuilder.Entity<BattleJudge>()
                .HasKey(bj => new { bj.BattleId, bj.JudgeId });

            modelBuilder.Entity<BattleJudge>()
                .HasOne(bj => bj.Battle)
                .WithMany(b => b.BattleJudges)
                .HasForeignKey(bj => bj.BattleId);

            modelBuilder.Entity<BattleJudge>()
                .HasOne(bj => bj.Judge)
                .WithMany(j => j.BattleJudges)
                .HasForeignKey(bj => bj.JudgeId);
        }
    }
}