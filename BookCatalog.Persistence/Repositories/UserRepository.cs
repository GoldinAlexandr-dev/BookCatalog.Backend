using BookCatalog.Application.Interfaces;
using BookCatalog.Domain.Entities;
using BookCatalog.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserWithReviewsAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.Reviews)
                    .ThenInclude(r => r.Book)
                        .ThenInclude(b => b.Author)
                .Include(u => u.Reviews)
                    .ThenInclude(r => r.Book)
                        .ThenInclude(b => b.Genres)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<User>> GetUsersWithReviewsAsync()
        {
            return await _dbSet
                .Include(u => u.Reviews)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbSet
                .AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet
                .AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string passwordHash)
        {
            return await _dbSet
                .AnyAsync(u => u.Username == username && u.PasswordHash == passwordHash);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _dbSet
                .Where(u => u.Role == role)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        // Переопределяем базовый метод для включения связанных данных
        public override async Task<User> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // Переопределяем метод получения всех пользователей с сортировкой
        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dbSet
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        // Дополнительный метод для поиска пользователей по имени
        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
        {
            return await _dbSet
                .Where(u => u.Username.Contains(searchTerm) ||
                           u.Email.Contains(searchTerm))
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        // Метод для обновления роли пользователя
        public async Task UpdateUserRoleAsync(int userId, string newRole)
        {
            var user = await GetByIdAsync(userId);
            if (user != null)
            {
                user.Role = newRole;
                await UpdateAsync(user);
            }
        }

        // Метод для получения статистики по пользователю
        public async Task<UserStats> GetUserStatsAsync(int userId)
        {
            var user = await _dbSet
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            return new UserStats
            {
                TotalReviews = user.Reviews.Count,
                AverageRating = user.Reviews.Any() ? user.Reviews.Average(r => r.Rating) : 0,
                LastReviewDate = user.Reviews.Any() ? user.Reviews.Max(r => r.CreatedAt) : null
            };
        }
    }

    // DTO для статистики пользователя
    public class UserStats
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public DateTime? LastReviewDate { get; set; }
    }
}
