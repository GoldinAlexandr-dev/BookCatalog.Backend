using BookCatalog.ApplicationServices.DTOs;

namespace BookCatalog.ApplicationServices.ServiceInterfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetAllBooksAsync();
        Task<BookDto> GetBookByIdAsync(int id);
        Task<BookDto> CreateBookAsync(CreateBookDto createBookDto);
        Task<BookDto> UpdateBookAsync(UpdateBookDto updateBookDto);
        Task DeleteBookAsync(int id);
        Task<IEnumerable<BookDto>> SearchBooksAsync(string searchTerm);
        Task<IEnumerable<BookDto>> GetBooksByAuthorNameAsync(string authorName);
        Task<IEnumerable<BookDto>> GetBooksByGenreNameAsync(string genreName);
        Task<BookSearchResultDto> SearchBooksAdvancedAsync(BookSearchDto searchDto);
    }
}
