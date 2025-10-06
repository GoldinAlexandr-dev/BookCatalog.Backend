using BookCatalog.Application.DTOs;

namespace BookCatalog.Application.ServiceInterfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync();
        Task<AuthorDetailDto> GetAuthorByIdAsync(int id);
        Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto);
        Task<AuthorDto> UpdateAuthorAsync(UpdateAuthorDto updateAuthorDto);  
        Task DeleteAuthorAsync(int id);
    }
}
