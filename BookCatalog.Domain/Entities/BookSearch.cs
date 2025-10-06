namespace BookCatalog.Domain.Entities
{
    public class BookSearch
    {
        public string? SearchTerm { get; set; }
        public string? AuthorName { get; set; }
        public string? GenreName { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
