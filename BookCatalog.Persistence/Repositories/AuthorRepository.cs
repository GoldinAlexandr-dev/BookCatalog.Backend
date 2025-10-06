using BookCatalog.Application.Exceptions;
using BookCatalog.Application.Interfaces;
using BookCatalog.Domain.Entities;
using BookCatalog.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Persistence.Repositories
{
    public class AuthorRepository : BaseRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Author>> GetAuthorsWithBooksAsync()
        {
            return await _dbSet
                .Include(a => a.Books)
                .ThenInclude(b => b.Genres)
                .ToListAsync();
        }

        public async Task<Author> GetAuthorWithDetailsAsync(int id)
        {
            var author = await _dbSet
                .Include(a => a.Books)
                    .ThenInclude(b => b.Genres)
                .Include(a => a.Books)
                    .ThenInclude(b => b.Reviews)
                .FirstOrDefaultAsync(a => a.Id == id);

            return author ?? throw new NotFoundException(nameof(Author), id);
        }
    }
}
