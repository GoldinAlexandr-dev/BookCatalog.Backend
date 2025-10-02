using BookCatalog.Application.Interfaces;
using BookCatalog.Domain.Entities;
using BookCatalog.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Persistence.Repositories
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetReviewsByBookAsync(int bookId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Book)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserAsync(int userId)
        {
            return await _dbSet
                .Include(r => r.Book)
                    .ThenInclude(b => b.Author)
                .Include(r => r.Book)
                    .ThenInclude(b => b.Genres)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsWithDetailsAsync()
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Book)
                    .ThenInclude(b => b.Author)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review> GetReviewWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Book)
                    .ThenInclude(b => b.Author)
                .Include(r => r.Book)
                    .ThenInclude(b => b.Genres)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<double> GetAverageRatingAsync(int bookId)
        {
            var averageRating = await _dbSet
                .Where(r => r.BookId == bookId)
                .AverageAsync(r => (double?)r.Rating) ?? 0.0;

            return Math.Round(averageRating, 1);
        }

        public async Task<int> GetReviewsCountAsync(int bookId)
        {
            return await _dbSet
                .Where(r => r.BookId == bookId)
                .CountAsync();
        }

        public async Task<bool> UserHasReviewedBookAsync(int userId, int bookId)
        {
            return await _dbSet
                .AnyAsync(r => r.UserId == userId && r.BookId == bookId);
        }

        // Переопределяем базовый метод для включения связанных данных
        public override async Task<Review> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Переопределяем метод получения всех отзывов с деталями
        public override async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await GetReviewsWithDetailsAsync();
        }

        public async Task<int> GetReviewsCountByUserAsync(int userId)
        {
            return await _dbSet
                .Where(r => r.UserId == userId)
                .CountAsync();
        }
    }
}
