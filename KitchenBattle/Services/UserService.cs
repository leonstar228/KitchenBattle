using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using KitchenBattle.Data;
using KitchenBattle.Models;

namespace KitchenBattle.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApplicationUser> GetOrCreateUserAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userName = user?.Identity?.Name;

            if (string.IsNullOrEmpty(userName)) return null!;
            var existingUser = await _db.ApplicationUser
                .Include(u => u.Recipes)
                .FirstOrDefaultAsync(u => u.UserName == userName);

            if (existingUser != null) return existingUser;
            var newUser = new ApplicationUser
            {
                UserName = userName,
                CreatedAt = DateTime.UtcNow
            };

            _db.ApplicationUser.Add(newUser);
            await _db.SaveChangesAsync();

            return newUser;
        }

        public async Task<string> GetCurrentUserIdAsync()
        {
            var user = await GetOrCreateUserAsync();
            return user?.Id.ToString() ?? "0";
        }

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }
    }
}