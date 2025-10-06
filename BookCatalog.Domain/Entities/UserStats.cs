namespace BookCatalog.Domain.Entities
{
    public class UserStats
    {
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public DateTime? LastReviewDate { get; set; }
    }
}
