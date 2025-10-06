using BookCatalog.Domain.Entities;

namespace BookCatalog.Application.Interfaces
{

    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByBookAsync(int bookId);
        Task<IEnumerable<Review>> GetReviewsByUserAsync(int userId);
        Task<IEnumerable<Review>> GetReviewsWithDetailsAsync();
        Task<Review> GetReviewWithDetailsAsync(int id);
        Task<double> GetAverageRatingAsync(int bookId);
        Task<bool> UserHasReviewedBookAsync(int userId, int bookId);
        Task<int> GetReviewsCountAsync(int bookId);
        Task<int> GetReviewsCountByUserAsync(int userId);
    }
}
