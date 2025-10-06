namespace BookCatalog.ApplicationServices.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
    }

    public class CreateReviewDto
    {
        public string Text { get; set; } = string.Empty;
        public int Rating { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
    }

    public class UpdateReviewDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}
