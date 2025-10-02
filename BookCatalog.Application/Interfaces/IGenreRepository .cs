using BookCatalog.Domain.Entities;

namespace BookCatalog.Application.Interfaces
{
    public interface IGenreRepository : IRepository<Genre>
    {
        Task<Genre> GetGenreWithBooksAsync(int genreId);
        Task<IEnumerable<Genre>> GetGenresWithBooksAsync();
        Task<Genre> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Genre>> GetPopularGenresAsync(int count);
        Task<IEnumerable<Genre>> SearchGenresAsync(string searchTerm);
        Task<int> GetBooksCountAsync(int genreId);
        Task<bool> IsGenreInUseAsync(int genreId);
        Task<IEnumerable<Genre>> GetGenresByIdsAsync(IEnumerable<int> genreIds);
    }
}
