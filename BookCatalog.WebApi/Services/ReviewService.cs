using AutoMapper;
using BookCatalog.Application.DTOs;
using BookCatalog.Application.Interfaces;
using BookCatalog.Application.Services;
using BookCatalog.Domain.Entities;

namespace BookCatalog.WebApi.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ReviewService(
            IReviewRepository reviewRepository,
            IBookRepository bookRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _reviewRepository.GetReviewsWithDetailsAsync();
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto> GetReviewByIdAsync(int id)
        {
            var review = await _reviewRepository.GetReviewWithDetailsAsync(id);
            if (review == null)
                throw new KeyNotFoundException($"Review with ID {id} not found");

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto)
        {
            // Проверяем существование книги
            var book = await _bookRepository.GetByIdAsync(createReviewDto.BookId);
            if (book == null)
                throw new ArgumentException($"Book with ID {createReviewDto.BookId} not found");

            // Проверяем существование пользователя
            var user = await _userRepository.GetByIdAsync(createReviewDto.UserId);
            if (user == null)
                throw new ArgumentException($"User with ID {createReviewDto.UserId} not found");

            // Проверяем, не оставлял ли пользователь уже отзыв на эту книгу
            var hasExistingReview = await _reviewRepository.UserHasReviewedBookAsync(
                createReviewDto.UserId, createReviewDto.BookId);

            if (hasExistingReview)
                throw new InvalidOperationException("User has already reviewed this book");

            // Проверяем валидность рейтинга
            if (createReviewDto.Rating < 1 || createReviewDto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            var review = _mapper.Map<Review>(createReviewDto);
            review.CreatedAt = DateTime.UtcNow;

            var createdReview = await _reviewRepository.AddAsync(review);
            return await GetReviewByIdAsync(createdReview.Id);
        }

        public async Task<ReviewDto> UpdateReviewAsync(UpdateReviewDto updateReviewDto)
        {
            var existingReview = await _reviewRepository.GetByIdAsync(updateReviewDto.Id);
            if (existingReview == null)
                throw new KeyNotFoundException($"Review with ID {updateReviewDto.Id} not found");

            // Проверяем валидность рейтинга
            if (updateReviewDto.Rating < 1 || updateReviewDto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            _mapper.Map(updateReviewDto, existingReview);
            await _reviewRepository.UpdateAsync(existingReview);

            return await GetReviewByIdAsync(existingReview.Id);
        }

        public async Task DeleteReviewAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                throw new KeyNotFoundException($"Review with ID {id} not found");

            await _reviewRepository.DeleteAsync(review);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByBookAsync(int bookId)
        {
            // Проверяем существование книги
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {bookId} not found");

            var reviews = await _reviewRepository.GetReviewsByBookAsync(bookId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(int userId)
        {
            // Проверяем существование пользователя
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var reviews = await _reviewRepository.GetReviewsByUserAsync(userId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<double> GetAverageRatingAsync(int bookId)
        {
            // Проверяем существование книги
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {bookId} not found");

            return await _reviewRepository.GetAverageRatingAsync(bookId);
        }

        public async Task<bool> UserHasReviewedBookAsync(int userId, int bookId)
        {
            // Проверяем существование пользователя и книги
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {bookId} not found");

            return await _reviewRepository.UserHasReviewedBookAsync(userId, bookId);
        }

        public async Task<int> GetReviewsCountAsync(int bookId)
        {
            // Проверяем существование книги
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {bookId} not found");

            return await _reviewRepository.GetReviewsCountAsync(bookId);
        }

        public async Task<IEnumerable<ReviewDto>> GetRecentReviewsAsync(int count)
        {
            var reviews = await _reviewRepository.GetReviewsWithDetailsAsync();
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews
                .OrderByDescending(r => r.CreatedAt)
                .Take(count));
        }
    }
}
