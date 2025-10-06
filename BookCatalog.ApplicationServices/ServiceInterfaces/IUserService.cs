using BookCatalog.ApplicationServices.DTOs;

namespace BookCatalog.ApplicationServices.ServiceInterfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDetailDto> GetUserByIdAsync(int id);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto);
        Task DeleteUserAsync(int id);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<bool> ValidateUserAsync(string username, string password);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role);
    }
}
