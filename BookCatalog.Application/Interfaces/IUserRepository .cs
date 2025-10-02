using BookCatalog.Domain.Entities;

namespace BookCatalog.Application.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetUserWithReviewsAsync(int userId);
        Task<IEnumerable<User>> GetUsersWithReviewsAsync();
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> ValidateCredentialsAsync(string username, string passwordHash);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
    }
}
