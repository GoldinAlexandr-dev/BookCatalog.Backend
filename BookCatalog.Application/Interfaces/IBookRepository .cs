using BookCatalog.Application.DTOs;
using BookCatalog.Domain.Entities;

namespace BookCatalog.Application.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> GetBooksWithDetailsAsync();
        Task<Book> GetBookWithDetailsAsync(int id);
        Task<IEnumerable<Book>> GetBooksByGenreAsync(int genreId);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId);
        Task<IEnumerable<Book>> SearchBooksAdvancedAsync(BookSearchDto searchDto);
        Task<IEnumerable<Book>> GetBooksByAuthorNameAsync(string authorName);
        Task<IEnumerable<Book>> GetBooksByGenreNameAsync(string genreName);
    }
}
