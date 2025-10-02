namespace BookCatalog.Application.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public string CoverImageUrl { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public double AverageRating { get; set; }
        public int ReviewsCount { get; set; }
    }

    public class CreateBookDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public string CoverImageUrl { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public List<int> GenreIds { get; set; } = new List<int>();
    }

    public class UpdateBookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public string CoverImageUrl { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public List<int> GenreIds { get; set; } = new List<int>();
    }

    public class BookSearchDto
    {
        public string? SearchTerm { get; set; }
        public string? AuthorName { get; set; }
        public string? GenreName { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class BookSearchResultDto
    {
        public IEnumerable<BookDto> Books { get; set; } = new List<BookDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
