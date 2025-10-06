using BookCatalog.ApplicationServices.DTOs;

namespace BookCatalog.ApplicationServices.ServiceInterfaces
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreDto>> GetAllGenresAsync();
        Task<GenreDetailDto> GetGenreByIdAsync(int id);
        Task<GenreDto> CreateGenreAsync(CreateGenreDto createGenreDto);
        Task<GenreDto> UpdateGenreAsync(UpdateGenreDto updateGenreDto);
        Task DeleteGenreAsync(int id);
        Task<bool> IsGenreInUseAsync(int genreId);
        Task<IEnumerable<GenreDto>> GetPopularGenresAsync(int count);
        Task<GenreDto> GetGenreByNameAsync(string name);
        Task<IEnumerable<GenreDto>> SearchGenresAsync(string searchTerm);
    }
}
