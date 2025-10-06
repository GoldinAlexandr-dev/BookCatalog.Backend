using AutoMapper;
using BookCatalog.Application.Exceptions;
using BookCatalog.Application.Interfaces;
using BookCatalog.ApplicationServices.DTOs;
using BookCatalog.ApplicationServices.ServiceInterfaces;
using BookCatalog.Domain.Entities;
using FluentValidation;

namespace BookCatalog.PersistenceServices.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateReviewDto> _createReviewValidator;
        private readonly IValidator<UpdateReviewDto> _updateReviewValidator;

        public ReviewService(
            IReviewRepository reviewRepository,
            IBookRepository bookRepository,
            IUserRepository userRepository,
            IMapper mapper,
            IValidator<CreateReviewDto> createReviewValidator,
            IValidator<UpdateReviewDto> updateReviewValidator)
        {
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _createReviewValidator = createReviewValidator;
            _updateReviewValidator = updateReviewValidator;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _reviewRepository.GetReviewsWithDetailsAsync();
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto> GetReviewByIdAsync(int id)
        {
            if (id <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "id", new[] { "Review ID must be greater than 0" } }
                });

            var review = await _reviewRepository.GetReviewWithDetailsAsync(id);
            if (review == null)
                throw new NotFoundException(nameof(Review), id);

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto createReviewDto)
        {
            // Валидация с помощью FluentValidation
            var validationResult = await _createReviewValidator.ValidateAsync(createReviewDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                throw new AppValidationException(errors);
            }

            // Проверяем существование книги
            var book = await _bookRepository.GetByIdAsync(createReviewDto.BookId);
            if (book == null)
                throw new NotFoundException(nameof(Book), createReviewDto.BookId);

            // Проверяем существование пользователя
            var user = await _userRepository.GetByIdAsync(createReviewDto.UserId);
            if (user == null)
                throw new NotFoundException(nameof(User), createReviewDto.UserId);

            // Проверяем, не оставлял ли пользователь уже отзыв на эту книгу
            var hasExistingReview = await _reviewRepository.UserHasReviewedBookAsync(
                createReviewDto.UserId, createReviewDto.BookId);

            if (hasExistingReview)
                throw new BusinessRuleException("User has already reviewed this book");

            var review = _mapper.Map<Review>(createReviewDto);
            review.CreatedAt = DateTime.UtcNow;

            var createdReview = await _reviewRepository.AddAsync(review);
            return await GetReviewByIdAsync(createdReview.Id);
        }

        public async Task<ReviewDto> UpdateReviewAsync(UpdateReviewDto updateReviewDto)
        {
            // Валидация с помощью FluentValidation
            var validationResult = await _updateReviewValidator.ValidateAsync(updateReviewDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                throw new AppValidationException(errors);
            }

            var existingReview = await _reviewRepository.GetByIdAsync(updateReviewDto.Id);
            if (existingReview == null)
                throw new NotFoundException(nameof(Review), updateReviewDto.Id);

            _mapper.Map(updateReviewDto, existingReview);
            await _reviewRepository.UpdateAsync(existingReview);

            return await GetReviewByIdAsync(existingReview.Id);
        }

        public async Task DeleteReviewAsync(int id)
        {
            if (id <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "id", new[] { "Review ID must be greater than 0" } }
                });

            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                throw new NotFoundException(nameof(Review), id);

            await _reviewRepository.DeleteAsync(review);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByBookAsync(int bookId)
        {
            if (bookId <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "bookId", new[] { "Book ID must be greater than 0" } }
                });

            // Проверяем существование книги
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                throw new NotFoundException(nameof(Book), bookId);

            var reviews = await _reviewRepository.GetReviewsByBookAsync(bookId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(int userId)
        {
            if (userId <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "userId", new[] { "User ID must be greater than 0" } }
                });

            // Проверяем существование пользователя
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException(nameof(User), userId);

            var reviews = await _reviewRepository.GetReviewsByUserAsync(userId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<double> GetAverageRatingAsync(int bookId)
        {
            if (bookId <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "bookId", new[] { "Book ID must be greater than 0" } }
                });

            // Проверяем существование книги
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                throw new NotFoundException(nameof(Book), bookId);

            return await _reviewRepository.GetAverageRatingAsync(bookId);
        }

        public async Task<bool> UserHasReviewedBookAsync(int userId, int bookId)
        {
            if (userId <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "userId", new[] { "User ID must be greater than 0" } }
                });

            if (bookId <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "bookId", new[] { "Book ID must be greater than 0" } }
                });

            // Проверяем существование пользователя и книги
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException(nameof(User), userId);

            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                throw new NotFoundException(nameof(Book), bookId);

            return await _reviewRepository.UserHasReviewedBookAsync(userId, bookId);
        }

        public async Task<int> GetReviewsCountAsync(int bookId)
        {
            if (bookId <= 0)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "bookId", new[] { "Book ID must be greater than 0" } }
                });

            // Проверяем существование книги
            var book = await _bookRepository.GetByIdAsync(bookId);
            if (book == null)
                throw new NotFoundException(nameof(Book), bookId);

            return await _reviewRepository.GetReviewsCountAsync(bookId);
        }

        public async Task<IEnumerable<ReviewDto>> GetRecentReviewsAsync(int count)
        {
            if (count <= 0 || count > 50)
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "count", new[] { "Count must be between 1 and 50" } }
                });

            var reviews = await _reviewRepository.GetReviewsWithDetailsAsync();
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews
                .OrderByDescending(r => r.CreatedAt)
                .Take(count));
        }
    }
}
