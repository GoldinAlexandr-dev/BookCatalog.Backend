namespace BookCatalog.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public string CoverImageUrl { get; set; } = string.Empty;

        // Связи
        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
