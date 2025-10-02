namespace BookCatalog.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;
        public int Rating { get; set; } // 1-5
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
