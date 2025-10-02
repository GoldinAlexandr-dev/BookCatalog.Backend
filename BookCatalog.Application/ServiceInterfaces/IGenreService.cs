using BookCatalog.Application.DTOs;

namespace BookCatalog.Application.Services
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
