using BookCatalog.Application.DTOs;

namespace BookCatalog.Application.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
        Task<ReviewDto> GetReviewByIdAsync(int id);
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto);
        Task<ReviewDto> UpdateReviewAsync(UpdateReviewDto updateReviewDto);
        Task DeleteReviewAsync(int id);
        Task<IEnumerable<ReviewDto>> GetReviewsByBookAsync(int bookId);
        Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(int userId);
        Task<double> GetAverageRatingAsync(int bookId);
        Task<bool> UserHasReviewedBookAsync(int userId, int bookId);
        Task<int> GetReviewsCountAsync(int bookId);
        Task<IEnumerable<ReviewDto>> GetRecentReviewsAsync(int count);
    }
}
