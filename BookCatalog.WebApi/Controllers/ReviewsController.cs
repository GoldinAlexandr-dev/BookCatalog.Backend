using BookCatalog.Application.DTOs;
using BookCatalog.Application.Exceptions;
using BookCatalog.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewDto>> GetReview(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            return Ok(review);
        }

        [HttpGet("book/{bookId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByBook(int bookId)
        {
            var reviews = await _reviewService.GetReviewsByBookAsync(bookId);
            return Ok(reviews);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewsByUser(int userId)
        {
            var reviews = await _reviewService.GetReviewsByUserAsync(userId);
            return Ok(reviews);
        }

        [HttpGet("recent/{count}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetRecentReviews(int count = 10)
        {
            var reviews = await _reviewService.GetRecentReviewsAsync(count);
            return Ok(reviews);
        }

        [HttpGet("book/{bookId}/average-rating")]
        public async Task<ActionResult<double>> GetAverageRating(int bookId)
        {
            var averageRating = await _reviewService.GetAverageRatingAsync(bookId);
            return Ok(averageRating);
        }

        [HttpGet("book/{bookId}/count")]
        public async Task<ActionResult<int>> GetReviewsCount(int bookId)
        {
            var count = await _reviewService.GetReviewsCountAsync(bookId);
            return Ok(count);
        }

        [HttpGet("user/{userId}/book/{bookId}/has-reviewed")]
        public async Task<ActionResult<bool>> UserHasReviewedBook(int userId, int bookId)
        {
            var hasReviewed = await _reviewService.UserHasReviewedBookAsync(userId, bookId);
            return Ok(hasReviewed);
        }

        [HttpPost]
        public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewDto createReviewDto)
        {
            var review = await _reviewService.CreateReviewAsync(createReviewDto);
            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ReviewDto>> UpdateReview(int id, [FromBody] UpdateReviewDto updateReviewDto)
        {
            if (id != updateReviewDto.Id)
            {
                throw new AppValidationException(new Dictionary<string, string[]>
                {
                    { "Id", new[] { "ID in route does not match ID in request body" } }
                });
            }

            var updatedReview = await _reviewService.UpdateReviewAsync(updateReviewDto);
            return Ok(updatedReview);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            await _reviewService.DeleteReviewAsync(id);
            return NoContent();
        }
    }
}
