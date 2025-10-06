using BookCatalog.Application.Exceptions;
using BookCatalog.Application.Interfaces;
using BookCatalog.Domain.Entities;
using BookCatalog.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Persistence.Repositories
{
    public class GenreRepository : BaseRepository<Genre>, IGenreRepository
    {
        public GenreRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Genre> GetGenreWithBooksAsync(int genreId)
        {
            var genre = await _dbSet
                .Include(g => g.Books)
                    .ThenInclude(b => b.Author)
                .Include(g => g.Books)
                    .ThenInclude(b => b.Reviews)
                .FirstOrDefaultAsync(g => g.Id == genreId);

            return genre ?? throw new NotFoundException(nameof(Genre), genreId);
        }

        public async Task<IEnumerable<Genre>> GetGenresWithBooksAsync()
        {
            return await _dbSet
                .Include(g => g.Books)
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

        public async Task<Genre> GetByNameAsync(string name)
        {
            var genre = await _dbSet
                .FirstOrDefaultAsync(g => g.Name == name);

            return genre ?? throw new NotFoundException(nameof(Genre), name);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet
                .AnyAsync(g => g.Name == name);
        }

        public async Task<IEnumerable<Genre>> GetPopularGenresAsync(int count)
        {
            return await _dbSet
                .Include(g => g.Books)
                .OrderByDescending(g => g.Books.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Genre>> SearchGenresAsync(string searchTerm)
        {
            return await _dbSet
                .Where(g => g.Name.Contains(searchTerm))
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

        public async Task<int> GetBooksCountAsync(int genreId)
        {
            var genre = await _dbSet
                .Include(g => g.Books)
                .FirstOrDefaultAsync(g => g.Id == genreId);

            return genre?.Books.Count ?? 0;
        }

        // Переопределяем базовый метод для включения связанных данных
        public override async Task<Genre> GetByIdAsync(int id)
        {
            var genre = await _dbSet
                .Include(g => g.Books)
                .FirstOrDefaultAsync(g => g.Id == id);

            return genre ?? throw new NotFoundException(nameof(Genre), id);
        }

        // Переопределяем метод получения всех жанров с сортировкой
        public override async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _dbSet
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

        // Дополнительный метод для получения жанров с количеством книг
        public async Task<IEnumerable<GenreWithBookCount>> GetGenresWithBookCountsAsync()
        {
            return await _dbSet
                .Select(g => new GenreWithBookCount
                {
                    Genre = g,
                    BookCount = g.Books.Count
                })
                .OrderByDescending(g => g.BookCount)
                .ThenBy(g => g.Genre != null ? g.Genre.Name : string.Empty)
                .ToListAsync();
        }

        // Метод для получения жанров по списку ID
        public async Task<IEnumerable<Genre>> GetGenresByIdsAsync(IEnumerable<int> genreIds)
        {
            return await _dbSet
                .Where(g => genreIds.Contains(g.Id))
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

        // Метод для проверки, используется ли жанр в каких-либо книгах
        public async Task<bool> IsGenreInUseAsync(int genreId)
        {
            var genre = await _dbSet
                .Include(g => g.Books)
                .FirstOrDefaultAsync(g => g.Id == genreId);

            return genre?.Books.Any() ?? false;
        }
    }
}
