namespace BookCatalog.Application.DTOs
{
    public class GenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int BooksCount { get; set; }
    }

    public class CreateGenreDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateGenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class GenreDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<BookDto> Books { get; set; } = new List<BookDto>();
    }
}
