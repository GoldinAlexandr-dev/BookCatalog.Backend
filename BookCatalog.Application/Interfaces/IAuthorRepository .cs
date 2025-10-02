using BookCatalog.Domain.Entities;

namespace BookCatalog.Application.Interfaces
{
    public interface IAuthorRepository : IRepository<Author>
    {
        Task<IEnumerable<Author>> GetAuthorsWithBooksAsync();
        Task<Author> GetAuthorWithDetailsAsync(int id);
    }
}
