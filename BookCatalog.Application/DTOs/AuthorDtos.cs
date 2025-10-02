namespace BookCatalog.Application.DTOs
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public int BooksCount { get; set; }
    }

    public class CreateAuthorDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
    }

    public class AuthorDetailDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public List<BookDto> Books { get; set; } = new List<BookDto>();
    }

    public class UpdateAuthorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
    }
}
