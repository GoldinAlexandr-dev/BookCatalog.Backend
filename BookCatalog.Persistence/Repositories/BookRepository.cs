using BookCatalog.Application.DTOs;
using BookCatalog.Application.Interfaces;
using BookCatalog.Domain.Entities;
using BookCatalog.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Persistence.Repositories
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Book>> GetBooksWithDetailsAsync()
        {
            return await _dbSet
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .Include(b => b.Reviews)
                .ToListAsync();
        }

        public async Task<Book> GetBookWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> GetBooksByGenreAsync(int genreId)
        {
            return await _dbSet
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .Where(b => b.Genres.Any(g => g.Id == genreId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            return await _dbSet
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .Where(b => b.Title.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId)
        {
            return await _dbSet
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .Where(b => b.AuthorId == authorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> SearchBooksAdvancedAsync(BookSearchDto searchDto)
        {
            var query = _dbSet
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .Include(b => b.Reviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            {
                query = query.Where(b => b.Title.Contains(searchDto.SearchTerm));
            }

            if (!string.IsNullOrEmpty(searchDto.AuthorName))
            {
                query = query.Where(b => b.Author.FullName.Contains(searchDto.AuthorName));
            }

            if (!string.IsNullOrEmpty(searchDto.GenreName))
            {
                query = query.Where(b => b.Genres.Any(g => g.Name.Contains(searchDto.GenreName)));
            }

            if (searchDto.MinYear.HasValue)
            {
                query = query.Where(b => b.PublicationYear >= searchDto.MinYear.Value);
            }

            if (searchDto.MaxYear.HasValue)
            {
                query = query.Where(b => b.PublicationYear <= searchDto.MaxYear.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorNameAsync(string authorName)
        {
            return await _dbSet
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .Where(b => b.Author.FullName.Contains(authorName))
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByGenreNameAsync(string genreName)
        {
            return await _dbSet
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .Where(b => b.Genres.Any(g => g.Name.Contains(genreName)))
                .ToListAsync();
        }
    }
}

