using BookCatalog.Application.DTOs;
using BookCatalog.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews()
        {
            try
            {
                var reviews = await _reviewService.GetAllReviewsAsync();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting reviews");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            try
            {
                var review = await _reviewService.GetReviewByIdAsync(id);
                return Ok(review);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting review with ID {ReviewId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("book/{bookId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByBook(int bookId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByBookAsync(bookId);
                return Ok(reviews);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting reviews for book ID {BookId}", bookId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByUser(int userId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByUserAsync(userId);
                return Ok(reviews);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting reviews for user ID {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("recent/{count}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetRecentReviews(int count = 10)
        {
            try
            {
                var reviews = await _reviewService.GetRecentReviewsAsync(count);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting recent reviews");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("book/{bookId}/average-rating")]
        public async Task<ActionResult<double>> GetAverageRating(int bookId)
        {
            try
            {
                var averageRating = await _reviewService.GetAverageRatingAsync(bookId);
                return Ok(averageRating);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting average rating for book ID {BookId}", bookId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("book/{bookId}/count")]
        public async Task<ActionResult<int>> GetReviewsCount(int bookId)
        {
            try
            {
                var count = await _reviewService.GetReviewsCountAsync(bookId);
                return Ok(count);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting reviews count for book ID {BookId}", bookId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("user/{userId}/book/{bookId}/has-reviewed")]
        public async Task<ActionResult<bool>> UserHasReviewedBook(int userId, int bookId)
        {
            try
            {
                var hasReviewed = await _reviewService.UserHasReviewedBookAsync(userId, bookId);
                return Ok(hasReviewed);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if user {UserId} reviewed book {BookId}", userId, bookId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ReviewDto>> CreateReview(CreateReviewDto createReviewDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var review = await _reviewService.CreateReviewAsync(createReviewDto);
                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating review");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, UpdateReviewDto updateReviewDto)
        {
            try
            {
                if (id != updateReviewDto.Id)
                    return BadRequest("ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _reviewService.UpdateReviewAsync(updateReviewDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating review with ID {ReviewId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting review with ID {ReviewId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
