namespace BookCatalog.Domain.Entities
{
    public class Author
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
