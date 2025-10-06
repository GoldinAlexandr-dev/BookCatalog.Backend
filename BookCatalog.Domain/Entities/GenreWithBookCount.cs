namespace BookCatalog.Domain.Entities
{
    public class GenreWithBookCount
    {
        public Genre? Genre { get; set; } = null!;
        public int BookCount { get; set; }
    }
}
